using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;
using KIU.LMS.Domain.Common.Models.Generating;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KIU.LMS.Application.Workers;

public class AssignmentGenerationWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AssignmentGenerationWorker> _logger;

    public AssignmentGenerationWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<AssignmentGenerationWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AssignmentGenerationWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var assignmentRepo = scope.ServiceProvider.GetRequiredService<IGeneratedAssignmentRepository>();
                var generator = scope.ServiceProvider.GetRequiredService<IQuestionGenerationService>();
                var questionRepo = scope.ServiceProvider.GetRequiredService<IGeneratedQuestionRepository>();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var assignment = await assignmentRepo.GetInProgressAsync(stoppingToken);
                if (assignment is null)
                {
                    await Task.Delay(2000, stoppingToken);
                    continue;
                }

                _logger.LogInformation("Started processing assignment: {Title}", assignment.Title);

                int needed = assignment.Count;
                int already = assignment.Questions.Count; 
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
                        assignment.Prompt,
                        quantity: remaining,
                        difficulty: assignment.Difficulty.ToString()
                    );

                    if (drafts is null || drafts.Count == 0)
                        continue;

                    foreach (var draft in drafts)
                    {
                        if (already >= needed)
                            break;

                        var validated = await ValidateWithModels(generator, draft, models, assignment.TaskContent, stoppingToken);
                        if (validated == null)
                            continue;

                        var newQ = new GeneratedQuestion(
                            Guid.NewGuid(),
                            assignment.CreateUserId,
                            assignment.Id,
                            validated.QuestionText,
                            validated.OptionA,
                            validated.OptionB,
                            validated.OptionC,
                            validated.OptionD,
                            validated.ExplanationCorrect,
                            validated.ExplanationIncorrect
                        );

                        await questionRepo.AddSafetyAsync(newQ, needed, assignment.Id);
                        already = assignment.Questions.Count; 
                        
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
                    _logger.LogWarning(concurrencyEx, "Concurrency conflict detected during SaveChanges. Retrying...");
    
                    foreach (var entry in concurrencyEx.Entries)
                    {
                        await entry.ReloadAsync(stoppingToken);
                    }

                    await uow.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AssignmentGenerationWorker");
            }

            await Task.Delay(2000, stoppingToken);
        }
    }

    private async Task<GeneratedQuestionDraft?> ValidateWithModels(
        IQuestionGenerationService generator,
        GeneratedQuestionDraft input,
        List<string> models,
        string taskContent,
        CancellationToken ct)
    {
        var current = input;

        if (models.Count == 1)
        {
            var res = await generator.ValidateAsync(models[0], taskContent ,current);
            if (res == null || (!res.IsValid && res.FixedQuestion == null))
                return null;

            return res.FixedQuestion ?? current;
        }

        for (int i = 1; i < models.Count; i++)
        {
            var res = await generator.ValidateAsync(models[i], taskContent,current);
            if (res == null) return null;
            if (!res.IsValid && res.FixedQuestion == null) return null;
            if (res.FixedQuestion != null) current = res.FixedQuestion;
        }

        return current;
    }
}
