namespace KIU.LMS.Domain.Entities.NoSQL;

[BsonCollection("questions")]
public class Question : Document
{
    public string QuestionBankId { get; private set; } = string.Empty;
    public QuestionType Type { get; private set; }
    public List<Option> Options { get; private set; } = new List<Option>();

    public Question() {}

    public Question(Guid questionBankId, QuestionType type, List<Option> options) 
    {
        QuestionBankId = questionBankId.ToString();
        Type = type;
        Options = options;
    }
}
