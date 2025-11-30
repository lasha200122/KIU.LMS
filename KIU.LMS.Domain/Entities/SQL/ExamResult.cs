namespace KIU.LMS.Domain.Entities.SQL;

public class ExamResult : Aggregate
{
    public Guid StudentId { get; private set; }
    public Guid QuizId { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset FinishedAt { get; private set; }
    public decimal Score { get; private set; }
    public int TotalQuestions { get; private set; }
    public int CorrectAnswers { get; private set; }
    public TimeSpan Duration { get; private set; }
    public string SessionId { get; private set; }
    public virtual Quiz Quiz { get; private set; } = null!;
    public virtual User User { get; private set; } = null!;

    private ExamResult() { }

    public ExamResult(
        Guid id,
        Guid studentId,
        Guid quizId,
        DateTimeOffset startedAt,
        DateTimeOffset finishedAt,
        decimal score,
        int totalQuestions,
        int correctAnswers,
        string sessionId,
        Guid userId)
        : base(id, DateTimeOffset.UtcNow, userId)
    {
        StudentId = studentId;
        QuizId = quizId;
        StartedAt = startedAt;
        FinishedAt = finishedAt;
        Score = score;
        TotalQuestions = totalQuestions;
        CorrectAnswers = correctAnswers;
        Duration = finishedAt - startedAt;
        SessionId = sessionId;
    }
    
    public void UpdateScore(decimal score, int correctAnswersCount)
    {
        Score = score;
        CorrectAnswers = correctAnswersCount;
        Update(CreateUserId, DateTimeOffset.UtcNow);
    }
}
