namespace KIU.LMS.Domain.Entities.NoSQL;

public class Option : Document
{
    public string Text { get; private set; } = string.Empty;
    public bool IsCorrect { get; private set; }

    public Option() {}

    public Option(string text, bool isCorrect) 
    {
        Text = text;
        IsCorrect = isCorrect;
    }    
}
