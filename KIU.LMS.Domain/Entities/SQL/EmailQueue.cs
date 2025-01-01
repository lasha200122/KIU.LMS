namespace KIU.LMS.Domain.Entities.SQL;

public class EmailQueue : Aggregate
{
    public Guid TemplateId { get; private set; }
    public string ToEmail { get; private set; } = null!;
    public string Variables { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public string? FailureReason { get; private set; }
    public DateTimeOffset? SentAt { get; private set; }
    public int RetryCount { get; private set; }

    public virtual EmailTemplate Template { get; private set; } = null!;

    public EmailQueue() { }

    public EmailQueue(
        Guid id,
        Guid templateId,
        string toEmail,
        string variables,
        Guid createUserId) : base(id, DateTimeOffset.Now, createUserId)
    {
        TemplateId = templateId;
        ToEmail = toEmail;
        Variables = variables;
        Status = "Pending";
        RetryCount = 0;
        Validate(this);
    }

    public void UpdateStatus(string status, string? failureReason = null)
    {
        Status = status;
        FailureReason = failureReason;
        if (status == "Sent")
        {
            SentAt = DateTimeOffset.Now;
        }
    }

    public void IncrementRetryCount()
    {
        RetryCount++;
    }

    private void Validate(EmailQueue queue)
    {
        if (queue.TemplateId == default)
            throw new Exception("შაბლონის ID სავალდებულოა");
        if (string.IsNullOrEmpty(queue.ToEmail))
            throw new Exception("მიმღების ელ-ფოსტა სავალდებულოა");
    }
}