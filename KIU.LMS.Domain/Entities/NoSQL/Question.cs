namespace KIU.LMS.Domain.Entities.NoSQL;

[BsonCollection("questions")]
public class Question : Document
{
    public string QuestionBankId { get; private set; } = string.Empty;
    public string Text { get; private set; }
    public string ExplanationCorrectAnswer { get; private set; } = string.Empty;
    public string ExplanationIncorrectAnswer { get; private set; } = string.Empty;
    public QuestionType Type { get; private set; }
    public List<Option> Options { get; private set; } = [];
    public Question() {}

    public Question(
        Guid questionBankId,
        string text,
        QuestionType type,
        List<Option> options,
        string explanationCorrectAnswer,
        string explanationIncorrectAnswer) 
    {
        QuestionBankId = questionBankId.ToString();
        Text = text;
        Type = type;
        Options = options;
        ExplanationCorrectAnswer = explanationCorrectAnswer;
        ExplanationIncorrectAnswer = explanationIncorrectAnswer;
    }
}
