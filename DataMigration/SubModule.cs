using System;
using System.Collections.Generic;

namespace DataMigration;

public partial class SubModule
{
    public Guid Id { get; set; }

    public Guid ModuleBankId { get; set; }

    public string? TaskDescription { get; set; }

    public string? CodeSolution { get; set; }

    public string? CodeGenerationPrompt { get; set; }

    public string? CodeGraidingPrompt { get; set; }

    public DateTimeOffset CreateDate { get; set; }

    public Guid CreateUserId { get; set; }

    public DateTimeOffset? LastUpdateDate { get; set; }

    public Guid? LastUpdateUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeleteDate { get; set; }

    public Guid? DeleteUserId { get; set; }

    public string? Solution { get; set; }

    public int? Difficulty { get; set; }

    public virtual ModuleBank ModuleBank { get; set; } = null!;
}
