using System;
using System.Collections.Generic;

namespace DataMigration;

public partial class ModuleBank
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid ModuleId { get; set; }

    public int Type { get; set; }

    public DateTimeOffset CreateDate { get; set; }

    public Guid CreateUserId { get; set; }

    public DateTimeOffset? LastUpdateDate { get; set; }

    public Guid? LastUpdateUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeleteDate { get; set; }

    public Guid? DeleteUserId { get; set; }

    public virtual Module Module { get; set; } = null!;

    public virtual ICollection<SubModule> SubModules { get; set; } = new List<SubModule>();
}
