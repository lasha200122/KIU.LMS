using KIU.LMS.Domain.Common.Enums.Assignment;
using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KIU.LMS.Application.Workers;

public class TaskGenerationWorker :  BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TaskGenerationWorker> _logger;

    public TaskGenerationWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<TaskGenerationWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TaskGenerationWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var assignmentRepo = scope.ServiceProvider.GetRequiredService<IGeneratedTaskRepository>();
                var generator = scope.ServiceProvider.GetRequiredService<ITaskGenerationService>();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var assignment = await assignmentRepo.GetTaskInProgressAsync(stoppingToken);
                if (assignment is null)
                {
                    await Task.Delay(2000, stoppingToken);
                    continue;
                }
                
                _logger.LogInformation("Started processing task assignment: {Title}, Type: {Type}", 
                    assignment.Title, assignment.Type);

                int needed = assignment.Count;
                int already = assignment.Tasks?.Count ?? 0;
                int maxAttempts = needed * 5;

                while (already < needed && maxAttempts-- > 0 && !stoppingToken.IsCancellationRequested)
                {
                    var models = assignment.Models;
                    if (models == null || models.Count == 0)
                    {
                        assignment.FailGeneration("No models provided");
                        break;
                    }

                    int remaining = needed - already;
                    var drafts = await generator.GenerateAsync(
                        models[0],
                        assignment.TaskContent,
                        quantity: remaining,
                        difficulty: assignment.Difficulty,
                        type: assignment.Type
                    );

                    if (drafts is null || drafts.Count == 0)
                        continue;

                    foreach (var draft in drafts)
                    {
                        if (already >= needed)
                            break;

                        var validated = await ValidateWithModels(
                            generator, 
                            draft, 
                            models, 
                            assignment.TaskContent, 
                            assignment.Type,
                            stoppingToken);
                        
                        if (validated == null)
                            continue;

                        var newTask = new GeneratedTask(
                            Guid.NewGuid(),
                            assignment.CreateUserId,
                            assignment.Id,
                            validated.TaskDescription,
                            validated.CodeSolution,
                            validated.CodeGenerationPrompt ?? string.Empty,
                            validated.CodeGradingPrompt
                        );

                        assignment.AddTask(newTask);
                        await assignmentRepo.AddSafetyAsync(newTask, needed, assignment.Id, stoppingToken);
                        already = assignment.Tasks?.Count ?? 0;
                        
                        if (already >= needed)
                            break;
                    }
                }

                if (already >= needed)
                    assignment.CompleteGeneration();
                else
                    assignment.FailGeneration("Generation incomplete");

                try
                {
                    await uow.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException concurrencyEx)
                {
                    _logger.LogWarning(concurrencyEx, 
                        "Concurrency conflict detected during SaveChanges. Retrying...");
    
                    foreach (var entry in concurrencyEx.Entries)
                    {
                        await entry.ReloadAsync(stoppingToken);
                    }

                    await uow.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TaskGenerationWorker");
            }

            await Task.Delay(2000, stoppingToken);
        }
    }

    private async Task<GeneratedTaskDraft?> ValidateWithModels(
        ITaskGenerationService generator,
        GeneratedTaskDraft input,
        List<string> models,
        string taskContent,
        GeneratedAssignmentType type,
        CancellationToken ct)
    {
        var current = input;

        if (models.Count == 1)
        {
            var res = await generator.ValidateAsync(models[0], taskContent, current, type);
            if (res == null || (!res.IsValid && res.FixedTask == null))
                return null;

            return res.FixedTask ?? current;
        }

        for (int i = 1; i < models.Count; i++)
        {
            var res = await generator.ValidateAsync(models[i], taskContent, current, type);
            if (res == null) return null;
            if (!res.IsValid && res.FixedTask == null) return null;
            if (res.FixedTask != null) current = res.FixedTask;
        }

        return current;
    }
}
