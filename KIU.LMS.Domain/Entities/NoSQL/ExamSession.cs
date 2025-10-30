namespace KIU.LMS.Domain.Entities.NoSQL;

[BsonCollection("examSessions")]
public class ExamSession : Document
{
    public string StudentId { get; private set; }
    public string QuizId { get; private set; }
    public List<ExamQuestion> Questions { get; private set; } = new();
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset? FinishedAt { get; private set; }
    public ExamStatus Status { get; private set; }
    public int CurrentQuestionIndex { get; private set; }
    public int? RemainingTimeInSeconds { get; private set; }
    public ExamQuestion? CurrentQuestion => CurrentQuestionIndex < Questions.Count ? Questions[CurrentQuestionIndex] : null;
    public bool IsExamTimeExpired => RemainingTimeInSeconds.HasValue && DateTimeOffset.UtcNow > StartedAt.AddSeconds(RemainingTimeInSeconds.Value);


    public ExamSession(
        string studentId,
        string quizId,
        List<ExamQuestion> questions,
        int? durationInSeconds = null)
    {
        if (questions == null || !questions.Any())
            throw new ArgumentException("Questions cannot be null or empty", nameof(questions));

        if (durationInSeconds.HasValue && durationInSeconds.Value <= 0)
            throw new ArgumentException("Duration must be positive", nameof(durationInSeconds));

        StudentId = studentId;
        QuizId = quizId;
        Questions = questions;
        StartedAt = DateTimeOffset.UtcNow;
        Status = ExamStatus.InProgress;
        CurrentQuestionIndex = 0;
        RemainingTimeInSeconds = durationInSeconds;
    }

    public bool CanAnswerCurrentQuestion()
    {
        if (CurrentQuestion == null)
            return false;

        if (!CurrentQuestion.TimeLimit.HasValue)
            return true;

        return CurrentQuestion.StartedAt.HasValue && !CurrentQuestion.IsTimeExpired;
    }

    public int? GetRemainingExamTime()
    {
        if (!RemainingTimeInSeconds.HasValue)
            return null;

        if (IsExamTimeExpired)
            return 0;

        var elapsedSeconds = (int)(DateTimeOffset.UtcNow - StartedAt).TotalSeconds;
        return Math.Max(0, RemainingTimeInSeconds.Value - elapsedSeconds);
    }

    public void UpdateExamStatus()
    {
        if (IsExamTimeExpired || CurrentQuestionIndex >= Questions.Count)
        {
            Status = ExamStatus.Completed;
            FinishedAt = DateTimeOffset.UtcNow;
        }
    }

    public void FinishExam() 
    {
        Status = ExamStatus.Completed;
        FinishedAt = DateTimeOffset.UtcNow;
    }

    public void InProggress() 
    {
        Status = ExamStatus.InProgress;
    }

    public void Paused()
    {
        Status = ExamStatus.Paused;
    }

    public bool StartQuestion()
    {
        if (CurrentQuestion == null || CurrentQuestion.StartedAt.HasValue)
            return false;

        Questions[CurrentQuestionIndex].StartedAt = DateTimeOffset.UtcNow;
        return true;
    }

    public void MoveToNextQuestion()
    {
        if (CurrentQuestionIndex < Questions.Count - 1)
        {
            CurrentQuestionIndex++;
        }
        else
        {
            Status = ExamStatus.Completed;
            FinishedAt = DateTimeOffset.UtcNow;
        }
    }
}

public class ExamQuestion
{
    public string QuestionId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public List<Option> Options { get; set; } = new();
    public string ExplanationCorrectAnswer { get; private set; } = string.Empty;
    public string ExplanationIncorrectAnswer { get; private set; } = string.Empty;
    public int? TimeLimit { get; set; }
    public DateTimeOffset? StartedAt { get; set; }

    public bool IsTimeExpired => TimeLimit.HasValue &&
        StartedAt.HasValue &&
        DateTimeOffset.UtcNow > StartedAt.Value.AddSeconds(TimeLimit.Value);

    public ExamQuestion() {}

    public ExamQuestion(
        string questionId,
        string text,
        string explanationCorrectAnswer,
        string explanationIncorrectAnswer,
        QuestionType type,
        List<Option> options,
        int? timeLimit,
        DateTimeOffset? startedAt)
    {
        QuestionId = questionId;
        Text = text;
        ExplanationCorrectAnswer = explanationCorrectAnswer;
        ExplanationIncorrectAnswer = explanationIncorrectAnswer;
        Type = type;
        Options = options;
        TimeLimit = timeLimit;
        StartedAt = startedAt;
    }

    public int? GetRemainingTime()
    {
        if (!TimeLimit.HasValue || !StartedAt.HasValue)
            return null;

        if (IsTimeExpired)
            return 0;

        var elapsedSeconds = (int)(DateTimeOffset.UtcNow - StartedAt.Value).TotalSeconds;
        return Math.Max(0, TimeLimit.Value - elapsedSeconds);
    }
}

public enum ExamStatus
{
    InProgress,
    Paused,
    Completed,
    Terminated
}