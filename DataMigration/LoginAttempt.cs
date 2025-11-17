using System;
using System.Collections.Generic;

namespace DataMigration;

public partial class LoginAttempt
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string IpAddress { get; set; } = null!;

    public string UserAgent { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string DeviceIdentifier { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public Guid CreateUserId { get; set; }

    public DateTimeOffset? LastUpdateDate { get; set; }

    public Guid? LastUpdateUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeleteDate { get; set; }

    public Guid? DeleteUserId { get; set; }

    public virtual User User { get; set; } = null!;
}
