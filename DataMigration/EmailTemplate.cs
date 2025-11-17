using System;
using System.Collections.Generic;

namespace DataMigration;

public partial class EmailTemplate
{
    public Guid Id { get; set; }

    public int Type { get; set; }

    public string Body { get; set; } = null!;

    public string Variables { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public Guid CreateUserId { get; set; }

    public DateTimeOffset? LastUpdateDate { get; set; }

    public Guid? LastUpdateUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeleteDate { get; set; }

    public Guid? DeleteUserId { get; set; }

    public virtual ICollection<EmailQueue> EmailQueues { get; set; } = new List<EmailQueue>();
}
