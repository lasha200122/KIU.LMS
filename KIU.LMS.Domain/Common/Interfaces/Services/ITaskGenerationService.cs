using KIU.LMS.Domain.Common.Enums.Assignment;

namespace KIU.LMS.Domain.Common.Interfaces.Services;

public interface ITaskGenerationService
{
    public Task<List<GeneratedTaskDraft>?> GenerateAsync(
        string model,
        string taskContent,
        int quantity,
        DifficultyType difficulty,
        GeneratedAssignmentType type);
    
    public Task<TaskValidationResult?> ValidateAsync(
        string model,
        string taskContent,
        GeneratedTaskDraft draft,
        GeneratedAssignmentType type);
}

public record GeneratedTaskDraft(
    string TaskDescription,
    string CodeSolution,
    string? CodeGenerationPrompt,
    string CodeGradingPrompt
);

public record TaskValidationResult(
    bool IsValid,
    string Reason,
    GeneratedTaskDraft? FixedTask
);