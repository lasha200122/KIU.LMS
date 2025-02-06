namespace KIU.LMS.Domain.Entities.NoSQL;

[BsonCollection("studentAnswers")]
public class StudentAnswer : Document
{
    public string SessionId { get; private set; } = string.Empty;
    public string QuestionId { get; private set; } = string.Empty;
    public List<string> SelectedOptions { get; private set; }
    public DateTimeOffset AnsweredAt { get; private set; }

    public StudentAnswer(string sessionId, string questionId, List<string> selectedOptions)
    {
        SessionId = sessionId;
        QuestionId = questionId;
        SelectedOptions = selectedOptions;
        AnsweredAt = DateTimeOffset.UtcNow;
    }
}