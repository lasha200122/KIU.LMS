using System;
using System.Collections.Generic;

namespace DataMigration;

public partial class ExamResult
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public Guid QuizId { get; set; }

    public DateTimeOffset StartedAt { get; set; }

    public DateTimeOffset FinishedAt { get; set; }

    public decimal Score { get; set; }

    public int TotalQuestions { get; set; }

    public int CorrectAnswers { get; set; }

    public TimeOnly Duration { get; set; }

    public DateTimeOffset CreateDate { get; set; }

    public Guid CreateUserId { get; set; }

    public DateTimeOffset? LastUpdateDate { get; set; }

    public Guid? LastUpdateUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeleteDate { get; set; }

    public Guid? DeleteUserId { get; set; }

    public string SessionId { get; set; } = null!;

    public virtual Quiz Quiz { get; set; } = null!;

    public virtual User Student { get; set; } = null!;
}
