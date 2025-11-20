using KIU.LMS.Domain.Common.Models;
using KIU.LMS.Domain.Common.Enums.Assignment;

namespace KIU.LMS.Domain.Entities.SQL;

public class GeneratedAssignment : Aggregate
{
    public string Title { get; private set; }
    public int Count { get; private set; }
    public List<string> Models { get; private set; }
    public string TaskContent { get; private set; }
    public DifficultyType Difficulty { get; private set; }
    public GeneratedAssignmentType Type { get; private set; }
    public string Prompt { get; private set; }
    public GeneratingStatus Status { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    private List<GeneratedQuestion>? _questions;
    public IReadOnlyCollection<GeneratedQuestion>? Questions => _questions?.AsReadOnly();
    
    private List<GeneratedTask>? _tasks;
    public IReadOnlyCollection<GeneratedTask>? Tasks => _tasks?.AsReadOnly();
    
    public GeneratedAssignment() { }
    
    public GeneratedAssignment(
        Guid id, Guid createUserId,
        string title, string taskContent,
        int count, DifficultyType difficulty,
        GeneratedAssignmentType type,
        string prompt,
        List<string> models) : base(id, DateTimeOffset.UtcNow, createUserId)
    {
        if (type == GeneratedAssignmentType.None)
            throw new ArgumentException("Assignment type must be specified.", nameof(type));

        Title = title;
        Count = count;
        TaskContent = taskContent;
        Type = type;
        Prompt = prompt;
        Models = models;
        Difficulty = difficulty;
        Status = GeneratingStatus.InProgress;
    }

    public void AddQuestion(GeneratedQuestion question)
    {
        if (Status != GeneratingStatus.InProgress)
            throw new InvalidOperationException("Questions can only be added while generation is in progress.");

        if (question is null)
            throw new ArgumentNullException(nameof(question));

        if (Type != GeneratedAssignmentType.MCQ)
            throw new InvalidOperationException("Questions can only be added to MCQ assignments.");

        if (_tasks != null && _tasks.Any())
            throw new InvalidOperationException("Cannot add questions when tasks are already present.");

        _questions ??=  [];
        _questions.Add(question);
        _tasks = null;
    }

    public void AddTask(GeneratedTask task)
    {
        if (Status != GeneratingStatus.InProgress)
            throw new InvalidOperationException("Tasks can only be added while generation is in progress.");

        if (task is null)
            throw new ArgumentNullException(nameof(task));

        if (Type is not (GeneratedAssignmentType.C2RS or GeneratedAssignmentType.IPEQ))
            throw new InvalidOperationException("Tasks can only be added to C2RS or IPEQ assignments.");

        if (_questions != null && _questions.Any())
            throw new InvalidOperationException("Cannot add tasks when questions are already present.");

        _tasks ??= new List<GeneratedTask>();
        _tasks.Add(task);
        _questions = null;
    }

    public void CompleteGeneration()
    {
        if (!HasValidContent())
            throw new InvalidOperationException($"Cannot complete generation without questions or tasks.");

        Status = GeneratingStatus.Completed;
        CompletedAt = DateTimeOffset.UtcNow;
    }

    public void FailGeneration(string errorMessage)
    {
        Status = GeneratingStatus.Failed;
    }

    private bool HasValidContent()
    {
        return (_questions != null && _questions.Any()) || (_tasks != null && _tasks.Any());
    }
}

public enum GeneratingStatus
{
    None = 0,
    InProgress = 1,
    Completed = 2,
    Failed = 3
}
