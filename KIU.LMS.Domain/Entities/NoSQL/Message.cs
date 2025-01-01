namespace KIU.LMS.Domain.Entities.NoSQL;

public class Message : Document
{
    public string Text { get; private set; } = string.Empty;
    public WriterType WriterType { get; private set; }

    public Message() {}

    public Message(string text, WriterType writerType) 
    {
        Text = text;
        WriterType = writerType;
    }
}
