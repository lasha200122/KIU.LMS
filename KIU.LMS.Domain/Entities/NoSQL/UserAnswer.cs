namespace KIU.LMS.Domain.Entities.NoSQL;

[BsonCollection("user_answers")]
public class UserAnswer : Document
{
    public string ExamId { get; private set; } = string.Empty;
    public string UserId { get; private set; } = string.Empty;
    public List<Answer> Answers { get; private set; } = new List<Answer>();

    public UserAnswer() {}

    public UserAnswer(Guid examId, Guid userId, List<Answer> answers) 
    {
        ExamId = examId.ToString();
        UserId = userId.ToString();
        Answers = answers;
    }
}
