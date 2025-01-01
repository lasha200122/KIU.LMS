namespace KIU.LMS.Domain.Entities.SQL;

public class Exam : Aggregate
{
    public Guid CourseId { get; private set; }
    public string Name { get; private set; } = null!;
    public ExamType Type { get; private set; }
    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset EndTime { get; private set; }
    public int DurationInMinutes { get; private set; }
    public int MaxScore { get; private set; }

    public virtual Course Course { get; private set; } = null!;

    private List<ExamQuestion> _questions = new();
    public IReadOnlyCollection<ExamQuestion> Questions => _questions;

    private List<ExamAttempt> _attempts = new();
    public IReadOnlyCollection<ExamAttempt> Attempts => _attempts;

    public Exam() { }

    public Exam(
        Guid id,
        Guid courseId,
        string name,
        ExamType type,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        int durationInMinutes,
        int maxScore,
        Guid createUserId) : base(id, DateTimeOffset.Now, createUserId)
    {
        CourseId = courseId;
        Name = name;
        Type = type;
        StartTime = startTime;
        EndTime = endTime;
        DurationInMinutes = durationInMinutes;
        MaxScore = maxScore;
        Validate(this);
    }

    private void Validate(Exam exam)
    {
        if (exam.CourseId == default)
            throw new Exception("კურსის ID სავალდებულოა");
        if (string.IsNullOrEmpty(exam.Name))
            throw new Exception("გამოცდის სახელი სავალდებულოა");
        if (exam.DurationInMinutes <= 0)
            throw new Exception("გამოცდის ხანგრძლივობა უნდა იყოს დადებითი რიცხვი");
        if (exam.StartTime >= exam.EndTime)
            throw new Exception("დაწყების დრო უნდა იყოს დასრულების დროზე ადრე");
    }
}