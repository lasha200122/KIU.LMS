namespace KIU.LMS.Domain.Entities.SQL;

public class Module : Aggregate
{
    public Guid CourseId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    
    public virtual Course Course { get; private set; } = null!;
    public virtual List<ModuleBank> ModuleBanks { get; private set; } = null!;
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

public class ModuleBank : Aggregate
{
    public string Name { get; private set; } = null!;
    public Guid ModuleId { get; private set; }
    public SubModuleType Type { get; private set; }

    public virtual Module Module { get; private set; } = null!;
    public virtual List<SubModule> SubModules { get; private set; } = null!;

    public ModuleBank() { }

    public ModuleBank(Guid id, string name, Guid moduleId, SubModuleType subModuleType, Guid userId)
        : base(id, DateTimeOffset.UtcNow, userId)
    {
        Name = name;
        ModuleId = moduleId;
        Type = subModuleType;
    }

    public void UModuleBank(string name, SubModuleType subModuleType, Guid userId)
    {
        Name = name;
        Type = subModuleType;
        Update(userId, DateTimeOffset.UtcNow);
    }
}

public class SubModule : Aggregate 
{
    public Guid ModuleBankId { get; private set; }
    public string? TaskDescription { get; private set; } 
    public string? CodeSolution { get; private set; } 
    public string? CodeGenerationPrompt { get; private set; }
    public string? CodeGraidingPrompt { get; private set; }
    public string? Solution { get; private set; }
    public DifficultyType? Difficulty { get; private set; } = null;

    public virtual ModuleBank ModuleBank { get; private set; } = null!;

    public SubModule() {}

    public SubModule(
        Guid id,
        Guid moduleBankId,
        string? taskDescription,
        string? codeSolution,
        string? codeGenerationPrompt,
        string? codeGraidingPrompt,
        string? solution,
        DifficultyType? difficulty,
        Guid userId) : base(id, DateTimeOffset.UtcNow, userId)
    {
        ModuleBankId = moduleBankId;
        TaskDescription = taskDescription;
        CodeSolution = codeSolution;
        CodeGenerationPrompt = codeGenerationPrompt;
        CodeGraidingPrompt = codeGraidingPrompt;
        Solution = solution;
        Difficulty = difficulty;
    }

    public void USub(
        string? taskDescription,
        string? codeSolution,
        string? codeGenerationPrompt,
        string? codeGraidingPrompt,
        string? solution,
        DifficultyType? difficulty, 
        Guid userId) 
    {
        TaskDescription = taskDescription;
        CodeSolution = codeSolution;
        CodeGenerationPrompt = codeGenerationPrompt;
        CodeGraidingPrompt = codeGraidingPrompt;
        Solution = solution;
        Difficulty = difficulty;

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

public enum DifficultyType
{
    None = 0,
    Easy = 1,
    Medium = 2,
    Hard = 3,
}