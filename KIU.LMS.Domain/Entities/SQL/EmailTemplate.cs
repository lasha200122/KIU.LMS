namespace KIU.LMS.Domain.Entities.SQL;

public class EmailTemplate : Aggregate
{
    public string Type { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public string Variables { get; private set; } = null!;
    public string Subject { get; private set; } = null!;

    private List<EmailQueue> _emailQueue = new();
    public IReadOnlyCollection<EmailQueue> EmailQueue => _emailQueue;

    public EmailTemplate() { }

    public EmailTemplate(
        Guid id,
        string type,
        string body,
        string variables,
        string subject,
        Guid createUserId) : base(id, DateTimeOffset.Now, createUserId)
    {
        Type = type;
        Body = body;
        Variables = variables;
        Subject = subject;
        Validate(this);
    }

    private void Validate(EmailTemplate template)
    {
        if (string.IsNullOrEmpty(template.Type))
            throw new Exception("შაბლონის ტიპი სავალდებულოა");
        if (string.IsNullOrEmpty(template.Body))
            throw new Exception("შაბლონის ტექსტი სავალდებულოა");
    }
}