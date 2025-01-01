namespace KIU.LMS.Domain.Entities.NoSQL;

[BsonCollection("conversations")]
public class Conversation : Document
{
    public string CourseId { get; private set; } = string.Empty;
    public string UserId { get; private set; } = string.Empty;
    public ConversationType Type { get; private set; }
    public List<Message> Messages { get; private set; } = new List<Message>();

    public Conversation() {}

    public Conversation(
        Guid courseId,
        Guid userId,
        ConversationType type,
        List<Message> messages) 
    {
        CourseId = courseId.ToString();
        UserId = userId.ToString();
        Type = type;
        Messages = messages;
    }
}
