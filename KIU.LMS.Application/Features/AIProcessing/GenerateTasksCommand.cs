using KIU.LMS.Domain.Common.Enums.Assignment;
using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public record GenerateTaskAssignmentCommand(Guid AssignmentId) : IRequest<AIProcessingResult>;
public class GenerateTaskAssignmentCommandHandler(
    IGeneratedAssignmentRepository generatedAssignmentRepository,
    IGeneratedTaskRepository generatedTaskRepository,
    ITaskGenerationService taskGenerationService,
    IUnitOfWork unitOfWork,
    ILogger<GenerateTaskAssignmentCommandHandler> logger)
    : IRequestHandler<GenerateTaskAssignmentCommand, AIProcessingResult>
{
    public async Task<AIProcessingResult> Handle(GenerateTaskAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await generatedAssignmentRepository.FirstOrDefaultWithTrackingIncludedAsync(
            x => x.Id == request.AssignmentId,
            [x => x.Questions]);

        if (assignment == null)
        {
            logger.LogWarning("Assignment {AssignmentId} not found", request.AssignmentId);
            return new AIProcessingResult(false, "{}", "Assignment not found");
        }

        if (assignment.Type is not (GeneratedAssignmentType.C2RS or GeneratedAssignmentType.IPEQ))
        {
            logger.LogWarning(
                "Assignment {AssignmentId} type {Type} is not supported for task generation",
                request.AssignmentId, assignment.Type);

            return new AIProcessingResult(false, "{}", "Invalid assignment type");
        }

        logger.LogInformation("Starting task generation for {Title}", assignment.Title);

        int needed = assignment.Count;
        int already = assignment.Tasks?.Count ?? 0;
        int maxAttempts = needed * 5;

        while (already < needed && maxAttempts-- > 0 && !cancellationToken.IsCancellationRequested)
        {
            var models = assignment.Models;
            if (models.Count == 0)
            {
                assignment.FailGeneration("No models provided");
                return new AIProcessingResult(false, "{}", "No models provided");
            }

            int remaining = needed - already;
            var drafts = await taskGenerationService.GenerateAsync(
                models[0],
                assignment.TaskContent,
                quantity: remaining,
                difficulty: assignment.Difficulty,
                type: assignment.Type);

            if (drafts == null || drafts.Count == 0)
                continue;

            foreach (var draft in drafts)
            {
                if (already >= needed) break;

                var validated = await ValidateWithModels(
                    taskGenerationService,
                    draft,
                    models,
                    assignment.TaskContent,
                    assignment.Type,
                    cancellationToken);

                if (validated == null) continue;

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

                await generatedTaskRepository.AddSafetyAsync(
                    newTask, needed, assignment.Id, cancellationToken);

                already = assignment.Tasks?.Count ?? 0;
            }
        }

        if (already >= needed)
        {
            assignment.CompleteGeneration();
            await unitOfWork.SaveChangesAsync();

            return new AIProcessingResult(
                true,
                $"{{\"generated\":{already}}}",
                null
            );
        }

        assignment.FailGeneration("Generation incomplete");
        await unitOfWork.SaveChangesAsync();

        return new AIProcessingResult(
            false,
            $"{{\"generated\":{already}}}",
            "Generation incomplete"
        );
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

        foreach (var model in models)
        {
            var res = await generator.ValidateAsync(model, taskContent, current, type);
            if (res == null) return null;
            if (!res.IsValid && res.FixedTask == null) return null;
            if (res.FixedTask != null) current = res.FixedTask;
        }

        return current;
    }
}

