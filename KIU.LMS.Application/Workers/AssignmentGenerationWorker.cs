using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;
using KIU.LMS.Domain.Common.Models.Generating;
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
                var questionRepo   = scope.ServiceProvider.GetRequiredService<IGeneratedQuestionRepository>();
                var generator      = scope.ServiceProvider.GetRequiredService<IQuestionGenerationService>();
                var uow            = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

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
                        assignment.FailGeneration("test");
                        break;
                    }

                    var drafts = await generator.GenerateAsync(
                        models[0],                    
                        assignment.TaskContent,
                        quantity: needed,                
                        difficulty: assignment.Difficulty.ToString()
                    );

                    if (drafts is null || drafts.Count == 0)
                        continue;

                    foreach (var draft in drafts)
                    {
                        var validated = await ValidateWithModels(generator, draft, models, stoppingToken);
                        if (validated == null) continue;

                        var newQ = new GeneratedQuestion(
                            assignment.Id,
                            validated.QuestionText,
                            validated.OptionA,
                            validated.OptionB,
                            validated.OptionC,
                            validated.OptionD,
                            validated.ExplanationCorrect,
                            validated.ExplanationIncorrect
                        );

                        assignment.AddQuestion(newQ);
                        already++;

                        if (already >= needed)
                            break;
                    }

                    await uow.SaveChangesAsync();
                }

                if (already >= needed)
                    assignment.CompleteGeneration();
                else
                    assignment.FailGeneration("test");

                await uow.SaveChangesAsync();
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
        CancellationToken ct)
    {
        var current = input;

        if (models.Count == 1)
        {
            var res = await generator.ValidateAsync(models[0], current);
            if (res == null || (!res.IsValid && res.FixedQuestion == null))
                return null;

            return res.FixedQuestion ?? current;
        }

        for (int i = 1; i < models.Count; i++)
        {
            var res = await generator.ValidateAsync(models[i], current);
            if (res == null) return null;
            if (!res.IsValid && res.FixedQuestion == null) return null;
            if (res.FixedQuestion != null) current = res.FixedQuestion;
        }

        return current;
    }
}