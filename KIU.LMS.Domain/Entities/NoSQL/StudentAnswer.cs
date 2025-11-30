namespace KIU.LMS.Domain.Entities.NoSQL;

[BsonCollection("studentAnswers")]
public class StudentAnswer : Document
{
    public string SessionId { get; private set; } = string.Empty;
    public string QuestionId { get; private set; } = string.Empty;
    
    // MCQ
    public List<string>? SelectedOptions { get; private set; }
    
    public string? StudentCode { get; private set; }
    public string? StudentPrompt { get; private set; }
    public string? GeneratedCode { get; private set; }
    public DateTimeOffset AnsweredAt { get; private set; }
    public StudentAnswer() { }

    public StudentAnswer(string sessionId, string questionId, List<string> selectedOptions)
    {
        SessionId = sessionId;
        QuestionId = questionId;
        SelectedOptions = selectedOptions;
        AnsweredAt = DateTimeOffset.UtcNow;
    }
    public StudentAnswer(string sessionId, string questionId, string studentCode, bool isCode)
    {
        SessionId = sessionId;
        QuestionId = questionId;
        StudentCode = studentCode;
        AnsweredAt = DateTimeOffset.UtcNow;
    }
    
    public StudentAnswer(string sessionId, string questionId, string studentPrompt)
    {
        SessionId = sessionId;
        QuestionId = questionId;
        StudentPrompt = studentPrompt;
        AnsweredAt = DateTimeOffset.UtcNow;
    }
    
    public void SetGeneratedCode(string code)
        => GeneratedCode = code;
}
