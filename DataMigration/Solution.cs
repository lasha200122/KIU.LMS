using System;
using System.Collections.Generic;

namespace DataMigration;

public partial class Solution
{
    public Guid Id { get; set; }

    public Guid AssignmentId { get; set; }

    public Guid UserId { get; set; }

    public string Value { get; set; } = null!;

    public string Grade { get; set; } = null!;

    public string FeedBack { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public Guid CreateUserId { get; set; }

    public DateTimeOffset? LastUpdateDate { get; set; }

    public Guid? LastUpdateUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeleteDate { get; set; }

    public Guid? DeleteUserId { get; set; }

    public int GradingStatus { get; set; }

    public virtual Assignment Assignment { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
