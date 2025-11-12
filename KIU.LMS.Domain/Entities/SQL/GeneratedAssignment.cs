using KIU.LMS.Domain.Common.Models;

namespace KIU.LMS.Domain.Entities.SQL;

public class GeneratedAssignment : Aggregate
{
    public string Title { get; private set; }
    public int Count { get; private set; }
    public List<string> Models { get; private set; }
    public string TaskContent { get; private set; }
    public DifficultyType Difficulty { get; private set; }
    public string Prompt { get; private set; }
    public GeneratingStatus Status { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    private List<GeneratedQuestion> _questions = new();
    public IReadOnlyCollection<GeneratedQuestion> Questions => _questions.AsReadOnly();


    public GeneratedAssignment() { }
    
    public GeneratedAssignment(
        Guid id, Guid createUserId,
        string title, string taskContent,
        int count, DifficultyType difficulty,
        string prompt,
        List<string> models) : base(id, DateTimeOffset.UtcNow, createUserId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        Title = title;
        Count = count;
        TaskContent = taskContent;
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

        _questions.Add(question);
    }

    public void CompleteGeneration()
    {
        if (!_questions.Any())
            throw new InvalidOperationException("Cannot complete generation without questions.");

        Status = GeneratingStatus.Completed;
        CompletedAt = DateTimeOffset.UtcNow;
    }

    public void FailGeneration(string errorMessage)
    {
        Status = GeneratingStatus.Failed;
    }
}

public enum GeneratingStatus
{
    None = 0,
    InProgress = 1,
    Completed = 2,
    Failed = 3
}
