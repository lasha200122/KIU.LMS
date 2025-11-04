namespace KIU.LMS.Domain.Common.Models.Generating;

public record GeneratedQuestionDraft(
    string QuestionText,
    string OptionA,
    string OptionB,
    string OptionC,
    string OptionD,
    string ExplanationCorrect,
    string ExplanationIncorrect
);
