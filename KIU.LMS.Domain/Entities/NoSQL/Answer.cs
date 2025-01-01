namespace KIU.LMS.Domain.Entities.NoSQL;

public class Answer : Document
{
    public string QuestionId { get; private set; } = string.Empty;
    public List<Option> Options { get; private set; } = new List<Option>();
    public string? Text { get; private set; }

    public Answer() {}

    public Answer(string questionId, List<Option> options, string? text) 
    {
        QuestionId = questionId;
        Options = options;
        Text = text;
    }
}
