using System;
using System.Collections.Generic;

namespace DataMigration;

public partial class Prompt
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public Guid CreateUserId { get; set; }

    public DateTimeOffset? LastUpdateDate { get; set; }

    public Guid? LastUpdateUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeleteDate { get; set; }

    public Guid? DeleteUserId { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}
