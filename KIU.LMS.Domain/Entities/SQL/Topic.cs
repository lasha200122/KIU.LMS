namespace KIU.LMS.Domain.Entities.SQL;

public class Topic : Aggregate
{
    public Guid CourseId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTimeOffset StartDateTime { get; private set; }
    public DateTimeOffset EndDateTime { get; private set; }

    public virtual Course Course { get; private set; } = null!;
    public List<Assignment> Assignments { get; private set; } = null!;

    public Topic() {}

    public Topic(Guid id, Guid courseId, string name, DateTimeOffset startDateTime, DateTimeOffset endDateTime, Guid userId) : base(id, DateTimeOffset.Now, userId)
    {
        CourseId = courseId;
        Name = name;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
    }
}
