namespace KIU.LMS.Domain.Entities.SQL;

public class Quiz : Aggregate
{
    public Guid CourseId { get; private set; }
    public Guid TopicId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public QuizType Type { get; private set; }
    public int Order { get; private set; }
    public int? Attempts { get; private set; }
    public DateTimeOffset StartDateTime { get; private set; }
    public DateTimeOffset? EndDateTime { get; private set; }
    public decimal? Score { get; private set; }
    public bool Explanation { get; private set; }
    public int? TimePerQuestion { get; private set; }

    public virtual Course Course { get; private set; } = null!;
    public virtual Topic Topic { get; private set; } = null!;

    private List<QuizBank> _quizBanks = new();
    public IReadOnlyCollection<QuizBank> QuizBanks => _quizBanks;

    private List<ExamResult> _examResults = new();
    public IReadOnlyCollection<ExamResult> ExamResults => _examResults;

    public Quiz() {}

    public Quiz(
        Guid id,
        Guid courseId,
        Guid topicId,
        string title,
        QuizType type,
        int order,
        int? attempts,
        DateTimeOffset startDateTime,
        DateTimeOffset? endDateTime,
        decimal? score,
        bool explanation,
        int? timePerQuestion,
        Guid userId) : base(id, DateTimeOffset.UtcNow, userId)
    {
        CourseId = courseId;
        TopicId = topicId;
        Title = title;
        Type = type;
        Order = order;
        Attempts = attempts;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        Score = score;
        Explanation = explanation;
        TimePerQuestion = timePerQuestion;
    }
}


public enum QuizType 
{
    None = 0,
    MCQ = 1,
    ExamPreparation = 2,
}