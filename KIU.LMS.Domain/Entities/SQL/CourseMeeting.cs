namespace KIU.LMS.Domain.Entities.SQL;

public class CourseMeeting : Aggregate
{
    public Guid CourseId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Url { get; private set; } = null!;
    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset EndTime { get; private set; }

    public virtual Course Course { get; private set; } = null!;

    public CourseMeeting() { }

    public CourseMeeting(
        Guid id,
        Guid courseId,
        string name,
        string url,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        Guid createUserId) : base(id, DateTimeOffset.Now, createUserId)
    {
        CourseId = courseId;
        Name = name;
        Url = url;
        StartTime = startTime;
        EndTime = endTime;
        Validate(this);
    }

    private void Validate(CourseMeeting meeting)
    {
        if (meeting.CourseId == default)
            throw new Exception("კურსის ID სავალდებულოა");
        if (string.IsNullOrEmpty(meeting.Name))
            throw new Exception("სახელი სავალდებულოა");
        if (meeting.StartTime >= meeting.EndTime)
            throw new Exception("დაწყების დრო უნდა იყოს დასრულების დროზე ადრე");
    }
}