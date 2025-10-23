namespace KIU.LMS.Domain.Entities.SQL;

public class AssignmentSolutionJob : Aggregate
{
    public Guid AssignmentId { get; private set; }
    public Guid SolutionId { get; private set; }
    public AssignmentSolutionJobStatus Status { get; private set; }
    public int Attempts { get; private set; }
    public string? Meta { get; private set; } = string.Empty; // {"ai":"mistral-small3.2:latest"}
    public string? Result { get; private set; } // {"grade":X,"feedback":"..."}

    public AssignmentSolutionJob()
    { }
    
    public AssignmentSolutionJob(Guid id, Guid assignmentId, Guid solutionId, string meta, Guid createUserId)
        : base(id, DateTimeOffset.UtcNow, createUserId)
    {
        AssignmentId = assignmentId;
        SolutionId = solutionId;
        Status = AssignmentSolutionJobStatus.Pending;
        Attempts = 0;
        Meta = meta;
    }

    public void MarkAsGraded(string result, Guid updateUserId)
    {
        Result = result;
        Status = AssignmentSolutionJobStatus.Graded;
        Update(updateUserId, DateTimeOffset.UtcNow);
    }

    public void MarkAsFailed(Guid updateUserId)
    {
        Status = AssignmentSolutionJobStatus.Failed;
        Update(updateUserId, DateTimeOffset.UtcNow);
    }

    public void IncrementAttempts(Guid updateUserId)
    {
        Attempts++;
        Update(updateUserId, DateTimeOffset.UtcNow);
    }
}

public enum AssignmentSolutionJobStatus
{
    None = 0,
    Pending = 1,
    Graded = 2,
    Failed = 3
}