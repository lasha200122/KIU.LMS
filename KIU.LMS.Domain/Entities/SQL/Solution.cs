namespace KIU.LMS.Domain.Entities.SQL;

public class Solution : Aggregate
{
    public Guid AssignmentId { get; private set; }
    public Guid UserId { get; private set; }
    public string Value { get; private set; } = string.Empty;
    public string Grade { get; private set; } = string.Empty;
    public string FeedBack { get; private set; } = string.Empty;
    public GradingStatus GradingStatus { get; private set; }

    public virtual Assignment Assignment { get; private set; } = null!;
    public virtual User User { get; private set; } = null!;

    public Solution() {}

    public Solution(
        Guid id,
        Guid assignmentId,
        Guid userId,
        string value,
        string grade,
        string feedBack,
        GradingStatus gradingStatus,
        Guid createUserId) : base(id, DateTimeOffset.UtcNow, createUserId)
    {
        AssignmentId = assignmentId;
        UserId = userId;
        Value = value;
        Grade = grade;
        FeedBack = feedBack;
        GradingStatus = gradingStatus;
    }

    public void Failed() 
    {
        GradingStatus = GradingStatus.Failed;
        FeedBack = "Automated grading failed. Requires manual review.";
    }

    public void Graded(string grade, string feedback) 
    {
        Grade = grade;
        FeedBack = feedback;
        GradingStatus = GradingStatus.Completed;
    }
}

public enum GradingStatus 
{
    None,
    InProgress,
    Completed,
    Failed
}