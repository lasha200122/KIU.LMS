using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;
using KIU.LMS.Domain.Common.Models.Generating;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public record GenerateAssignmentCommand(Guid GeneratedAssignmentId) : IRequest<AIProcessingResult>;

public class GenerateAssignmentCommandHandler : IRequestHandler<GenerateAssignmentCommand, AIProcessingResult>
{
    private readonly IGeneratedAssignmentRepository _assignmentRepo;
    private readonly IQuestionGenerationService _generator;
    private readonly IGeneratedQuestionRepository _questionRepo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<GenerateAssignmentCommandHandler> _logger;

    public GenerateAssignmentCommandHandler(
        IGeneratedAssignmentRepository assignmentRepo,
        IQuestionGenerationService generator,
        IGeneratedQuestionRepository questionRepo,
        IUnitOfWork uow,
        ILogger<GenerateAssignmentCommandHandler> logger)
    {
        _assignmentRepo = assignmentRepo;
        _generator = generator;
        _questionRepo = questionRepo;
        _uow = uow;
        _logger = logger;
    }

    public async Task<AIProcessingResult> Handle(GenerateAssignmentCommand request, CancellationToken cancellationToken)
{
    var assignment = await _assignmentRepo.FirstOrDefaultWithTrackingIncludedAsync(
        x => x.Id == request.GeneratedAssignmentId,
        includeProperties: [x => x.Questions]);

    if (assignment is null)
    {
        _logger.LogWarning("Assignment {AssignmentId} not found", request.GeneratedAssignmentId);
        return new AIProcessingResult(false, "{}", "Assignment not found");
    }

    _logger.LogInformation("Started processing assignment: {Title}", assignment.Title);

    var needed = assignment.Count;
    var already = assignment.Questions.Count;
    var maxAttempts = needed * 5;

    while (already < needed && maxAttempts-- > 0 && !cancellationToken.IsCancellationRequested)
    {
        if (assignment.Models.Count == 0)
        {
            assignment.FailGeneration("No models provided");
            return new AIProcessingResult(false, "{}", "No models provided");
        }

        var drafts = await _generator.GenerateAsync(
            assignment.Models[0],
            assignment.Prompt,
            quantity: needed - already,
            difficulty: assignment.Difficulty.ToString()
        );

        if (drafts is null || drafts.Count == 0)
            continue;

        foreach (var draft in drafts)
        {
            if (already >= needed) break;

            var validated = await ValidateWithModels(
                _generator, draft, assignment.Models, assignment.TaskContent, cancellationToken);
            if (validated == null) continue;

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

            await _questionRepo.AddSafetyAsync(newQ, needed, assignment.Id);
            already = assignment.Questions.Count;

            if (already >= needed) break;
        }
    }

    if (already >= needed)
    {
        assignment.CompleteGeneration();
        await _uow.SaveChangesAsync();

        return new AIProcessingResult(
            true,
            $"{{\"generated\":{already}}}",
            null
        );
    }

    assignment.FailGeneration("Generation incomplete");
    await _uow.SaveChangesAsync();

    return new AIProcessingResult(
        false,
        $"{{\"generated\":{already}}}",
        "Generation incomplete"
    );
}


    private async Task<GeneratedQuestionDraft?> ValidateWithModels(
        IQuestionGenerationService generator,
        GeneratedQuestionDraft input,
        List<string> models,
        string taskContent,
        CancellationToken ct)
    {
        var current = input;

        foreach (var model in models)
        {
            var res = await generator.ValidateAsync(model, taskContent, current);
            if (res == null) return null;

            if (!res.IsValid && res.FixedQuestion == null)
                return null;

            if (res.FixedQuestion != null)
                current = res.FixedQuestion;
        }
        
        return current;
    }

}