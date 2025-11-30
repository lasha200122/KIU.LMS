namespace KIU.LMS.Domain.Common.Models.Excel;

public sealed class QuestionExcelDto
{
    public string Question { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public List<string> IncorrectAnswers { get; set; } = new();
    
    public string ExplanationCorrectAnswer { get; private set; } = string.Empty;
    public string ExplanationIncorrectAnswer { get; private set; } = string.Empty;

    public QuestionExcelDto(
        string question, 
        string correctAnswer, 
        List<string> incorrectAnswers,
        string explanationCorrectAnswer,
        string explanationIncorrectAnswer)
    {
        Question = question;
        CorrectAnswer = correctAnswer;
        IncorrectAnswers = incorrectAnswers;
        ExplanationCorrectAnswer = explanationCorrectAnswer;
        ExplanationIncorrectAnswer = explanationIncorrectAnswer;
    }
}
