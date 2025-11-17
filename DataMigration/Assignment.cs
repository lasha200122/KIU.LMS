using System;
using System.Collections.Generic;

namespace DataMigration;

public partial class Assignment
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    public int Type { get; set; }

    public string Name { get; set; } = null!;

    public int Order { get; set; }

    public DateTimeOffset? StartDateTime { get; set; }

    public DateTimeOffset? EndDateTime { get; set; }

    public decimal? Score { get; set; }

    public string? Problem { get; set; }

    public string? Code { get; set; }

    public string? FileName { get; set; }

    public bool IsPublic { get; set; }

    public DateTimeOffset CreateDate { get; set; }

    public Guid CreateUserId { get; set; }

    public DateTimeOffset? LastUpdateDate { get; set; }

    public Guid? LastUpdateUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeleteDate { get; set; }

    public Guid? DeleteUserId { get; set; }

    public Guid TopicId { get; set; }

    public bool Aigrader { get; set; }

    public int SolutionType { get; set; }

    public Guid? PromptId { get; set; }

    public bool FullScreen { get; set; }

    public int? RuntimeAttempt { get; set; }

    public bool IsTraining { get; set; }

    public string? GraidingPrompt { get; set; }

    public string? PromptText { get; set; }

    public string? CodeSolution { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Prompt? Prompt { get; set; }

    public virtual ICollection<Solution> Solutions { get; set; } = new List<Solution>();

    public virtual Topic Topic { get; set; } = null!;
}
