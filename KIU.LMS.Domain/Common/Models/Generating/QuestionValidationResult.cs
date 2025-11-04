namespace KIU.LMS.Domain.Common.Models.Generating;

public record QuestionValidationResult(
    bool IsValid,
    string Reason,
    GeneratedQuestionDraft? FixedQuestion
);