namespace KIU.LMS.Domain.Entities.SQL;

public class ExamAttempt : Aggregate
{
    public Guid UserId { get; private set; }
    public Guid ExamId { get; private set; }
    public int AttemptNumber { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    public virtual User User { get; private set; } = null!;
    public virtual Exam Exam { get; private set; } = null!;

    public ExamAttempt() { }

    public ExamAttempt(
        Guid id,
        Guid userId,
        Guid examId,
        int attemptNumber,
        Guid createUserId) : base(id, DateTimeOffset.Now, createUserId)
    {
        UserId = userId;
        ExamId = examId;
        AttemptNumber = attemptNumber;
        StartedAt = DateTimeOffset.Now;
        Validate(this);
    }

    public void Complete()
    {
        CompletedAt = DateTimeOffset.Now;
    }

    private void Validate(ExamAttempt attempt)
    {
        if (attempt.UserId == default)
            throw new Exception("მომხმარებლის ID სავალდებულოა");
        if (attempt.ExamId == default)
            throw new Exception("გამოცდის ID სავალდებულოა");
        if (attempt.AttemptNumber <= 0)
            throw new Exception("მცდელობის ნომერი უნდა იყოს დადებითი რიცხვი");
    }
}