namespace KIU.LMS.Domain.Entities.SQL;

public class Module : Aggregate
{
    public Guid CourseId { get; private set; }
    public string Name { get; private set; } = string.Empty;


    public virtual Course Course { get; private set; } = null!;
    public virtual List<SubModule> SubModules { get; private set; } = null!;
    private List<QuestionBank> _questionBanks = new();
    public IReadOnlyCollection<QuestionBank> QuestionBanks => _questionBanks;


    public Module() {}

    public Module(Guid id, Guid courseId, string name, Guid userId) : base(id, DateTimeOffset.UtcNow, userId) 
    {
        CourseId = courseId;
        Name = name;
    }

    public void UModule(string name, Guid userId) 
    {
        Name = name;

        Update(userId, DateTimeOffset.UtcNow);
    }
}


public class SubModule : Aggregate 
{
    public Guid ModuleId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Problem { get; private set; }
    public string? Code { get; private set; }
    public SubModuleType SubModuleType { get; private set; }
    public Guid? PromptId { get; private set; }

    public virtual Module Module { get; private set; } = null!;

    public SubModule() {}

    public SubModule(
        Guid id,
        Guid moduleId,
        string name,
        string? problem,
        string? code,
        SubModuleType subModuleType,
        Guid? promptId,
        Guid userId) : base(id, DateTimeOffset.UtcNow, userId)
    {
        ModuleId = moduleId;
        Name = name;
        Problem = problem;
        Code = code;
        SubModuleType = subModuleType;
        PromptId = promptId;
    }

    public void USub(string name, string? problem, string? code, Guid? promptId ,Guid userId) 
    {
        Name = name;
        Problem = problem;
        Code = code;
        PromptId = promptId;

        Update(userId, DateTimeOffset.UtcNow);
    }
}

public enum SubModuleType 
{
    None = 0,
    Homework = 1,
    Classwork = 2,
    IPEQ = 3,
    MCQ = 4,
    Project = 5,
    C2RS = 6,
}