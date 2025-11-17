namespace KIU.LMS.Domain.Entities.SQL;

public class GeneratedTask : Aggregate
{
    public Guid GeneratedAssignmentId { get; private set; }
    public string TaskDescription { get; private set; }
    public string CodeSolution { get; private set; }
    public string CodeGenerationPrompt { get; private set; }
    public string CodeGradingPrompt { get; private set; }
    public GeneratedAssignment Assignment { get; private set; } = null!;
    
    public GeneratedTask(){}

    public GeneratedTask(Guid newGuid, Guid createdUserid,
        Guid generatedAssignmentId, string taskDescription,
        string codeSolution, string codeGenerationPrompt, string codeGradingPrompt)
        : base(newGuid, DateTimeOffset.UtcNow, createdUserid)
    {
        GeneratedAssignmentId =  generatedAssignmentId;
        TaskDescription = taskDescription;
        CodeSolution = codeSolution;
        CodeGenerationPrompt = codeGenerationPrompt;
        CodeGradingPrompt = codeGradingPrompt;
    }
}