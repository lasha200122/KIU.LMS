using System;
using System.Collections.Generic;

namespace DataMigration;

public partial class EmailQueue
{
    public Guid Id { get; set; }

    public Guid TemplateId { get; set; }

    public string ToEmail { get; set; } = null!;

    public string Variables { get; set; } = null!;

    public int Status { get; set; }

    public string? FailureReason { get; set; }

    public DateTimeOffset? SentAt { get; set; }

    public int RetryCount { get; set; }

    public DateTimeOffset CreateDate { get; set; }

    public Guid CreateUserId { get; set; }

    public DateTimeOffset? LastUpdateDate { get; set; }

    public Guid? LastUpdateUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeleteDate { get; set; }

    public Guid? DeleteUserId { get; set; }

    public virtual EmailTemplate Template { get; set; } = null!;
}
