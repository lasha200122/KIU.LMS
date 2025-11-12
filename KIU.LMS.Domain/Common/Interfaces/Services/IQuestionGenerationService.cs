using KIU.LMS.Domain.Common.Models.Generating;

namespace KIU.LMS.Domain.Common.Interfaces.Services;

public interface IQuestionGenerationService 
{
    Task<List<GeneratedQuestionDraft>?> GenerateAsync(
        string model,
        string prompt,
        int quantity,
        string difficulty);
    
    Task<QuestionValidationResult?> ValidateAsync(
        string model,
        string taskContent,
        GeneratedQuestionDraft draft);
}