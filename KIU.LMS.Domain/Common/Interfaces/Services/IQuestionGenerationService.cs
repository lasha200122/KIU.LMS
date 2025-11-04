using KIU.LMS.Domain.Common.Models.Generating;

namespace KIU.LMS.Domain.Common.Interfaces.Services;

public interface IQuestionGenerationService 
{
    Task<List<GeneratedQuestionDraft>?> GenerateAsync(
        string model,
        string taskContent,
        int quantity,
        string difficulty);
    
    Task<QuestionValidationResult?> ValidateAsync(
        string model,
        GeneratedQuestionDraft draft);
}