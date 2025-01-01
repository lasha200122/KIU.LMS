namespace KIU.LMS.Domain.Entities.SQL;

public class ExamConfiguration : Aggregate
{
    public Guid ExamId { get; private set; }
    public int Attempts { get; private set; }
    public int LateMinutes { get; private set; }
    public int ReconnectAttempts { get; private set; }
    public int ReconnectMinutes { get; private set; }
    public bool EnableIpRestriction { get; private set; }
    public string AllowedIpRanges { get; private set; } = null!;

    public virtual Exam Exam { get; private set; } = null!;

    public ExamConfiguration() { }

    public ExamConfiguration(
        Guid id,
        Guid examId,
        int attempts,
        int lateMinutes,
        int reconnectAttempts,
        int reconnectMinutes,
        bool enableIpRestriction,
        string allowedIpRanges,
        Guid createUserId) : base(id, DateTimeOffset.Now, createUserId)
    {
        ExamId = examId;
        Attempts = attempts;
        LateMinutes = lateMinutes;
        ReconnectAttempts = reconnectAttempts;
        ReconnectMinutes = reconnectMinutes;
        EnableIpRestriction = enableIpRestriction;
        AllowedIpRanges = allowedIpRanges;
        Validate(this);
    }

    private void Validate(ExamConfiguration config)
    {
        if (config.ExamId == default)
            throw new Exception("გამოცდის ID სავალდებულოა");
        if (config.Attempts < 1)
            throw new Exception("მცდელობების რაოდენობა უნდა იყოს მინიმუმ 1");
        if (config.LateMinutes < 0)
            throw new Exception("დაგვიანების დრო არ შეიძლება იყოს უარყოფითი");
        if (config.ReconnectAttempts < 0)
            throw new Exception("რეკონექთის მცდელობები არ შეიძლება იყოს უარყოფითი");
    }
}