namespace KIU.LMS.Domain.Entities.SQL;

public class Assignment : Aggregate
{
    public Guid CourseId { get; private set; }
    public Guid TopicId { get; private set; }
    public AssignmentType Type { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Order { get; private set; }
    public DateTimeOffset? StartDateTime { get; private set; }
    public DateTimeOffset? EndDateTime { get; private set; }
    public decimal? Score { get; private set; }
    public string? Problem { get; private set; }
    public string? Code { get; private set; }
    public string? FileName { get; private set; }
    public bool IsPublic { get; private set; }
    public bool AIGrader { get; private set; }
    public SolutionType SolutionType { get; private set; }


    public virtual Course Course { get; private set; } = null!;
    public virtual Topic Topic { get; private set; } = null!;

    public virtual List<Solution> Solutions { get; private set; } = null!;
    public Assignment() {}

    public Assignment(
        Guid id,
        Guid courseId,
        Guid topicId,
        AssignmentType type,
        string name,
        int order,
        DateTimeOffset? startDateTime,
        DateTimeOffset? endDateTime,
        decimal? score,
        string? problem,
        string? code,
        string? fileName,
        bool isPublic,
        bool aiGrader,
        SolutionType solutionType,
        Guid createUserId) : base(id, DateTimeOffset.Now, createUserId)
    {
        CourseId = courseId;
        TopicId = topicId;
        Type = type;
        Name = name;
        Order = order;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        Score = score;
        Problem = problem;
        Code = code;
        FileName = fileName;
        IsPublic = isPublic;
        AIGrader = aiGrader;
        SolutionType = solutionType;
    }
}


public enum AssignmentType 
{
    None,
    Homework,
    ClassWork,
    IPEQ,
    Project
}

public enum SolutionType 
{
    None,
    Text,
    Code,
    File
}