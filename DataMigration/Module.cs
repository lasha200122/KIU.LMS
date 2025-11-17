using System;
using System.Collections.Generic;

namespace DataMigration;

public partial class Module
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    public string Name { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public Guid CreateUserId { get; set; }

    public DateTimeOffset? LastUpdateDate { get; set; }

    public Guid? LastUpdateUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeleteDate { get; set; }

    public Guid? DeleteUserId { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<ModuleBank> ModuleBanks { get; set; } = new List<ModuleBank>();

    public virtual ICollection<QuestionBank> QuestionBanks { get; set; } = new List<QuestionBank>();
}
