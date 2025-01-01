namespace KIU.LMS.Domain.Entities.NoSQL;

[BsonCollection("user_exams")]
public class UserExam : Document
{
    public string ExamId { get; private set; } = string.Empty;
    public string UserId { get; private set; } = string.Empty;
    public List<Question> Questions { get; private set; } = new List<Question>();

    public UserExam() {}

    public UserExam(Guid examId, Guid userId, List<Question> questions) 
    {
        ExamId = examId.ToString();
        UserId = userId.ToString();
        Questions = questions;
    }
}
