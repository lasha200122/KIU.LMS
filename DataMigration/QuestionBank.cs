using System;
using System.Collections.Generic;

namespace DataMigration;

public partial class QuestionBank
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public Guid CreateUserId { get; set; }

    public DateTimeOffset? LastUpdateDate { get; set; }

    public Guid? LastUpdateUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeleteDate { get; set; }

    public Guid? DeleteUserId { get; set; }

    public Guid ModuleId { get; set; }

    public virtual Module Module { get; set; } = null!;

    public virtual ICollection<QuizBank> QuizBanks { get; set; } = new List<QuizBank>();
}
