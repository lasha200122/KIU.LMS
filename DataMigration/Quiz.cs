using System;
using System.Collections.Generic;

namespace DataMigration;

public partial class Quiz
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    public Guid TopicId { get; set; }

    public string Title { get; set; } = null!;

    public int Type { get; set; }

    public int Order { get; set; }

    public int? Attempts { get; set; }

    public DateTimeOffset StartDateTime { get; set; }

    public DateTimeOffset? EndDateTime { get; set; }

    public decimal? Score { get; set; }

    public DateTimeOffset CreateDate { get; set; }

    public Guid CreateUserId { get; set; }

    public DateTimeOffset? LastUpdateDate { get; set; }

    public Guid? LastUpdateUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeleteDate { get; set; }

    public Guid? DeleteUserId { get; set; }

    public bool Explanation { get; set; }

    public int? TimePerQuestion { get; set; }

    public decimal? MinusScore { get; set; }

    public DateTimeOffset? PublicTill { get; set; }

    public bool IsTraining { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();

    public virtual ICollection<QuizBank> QuizBanks { get; set; } = new List<QuizBank>();

    public virtual Topic Topic { get; set; } = null!;
}
