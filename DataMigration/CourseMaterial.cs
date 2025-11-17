using System;
using System.Collections.Generic;

namespace DataMigration;

public partial class CourseMaterial
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    public Guid? CourseMaterialParentId { get; set; }

    public string Name { get; set; } = null!;

    public string Url { get; set; } = null!;

    public int Order { get; set; }

    public DateTimeOffset CreateDate { get; set; }

    public Guid CreateUserId { get; set; }

    public DateTimeOffset? LastUpdateDate { get; set; }

    public Guid? LastUpdateUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeleteDate { get; set; }

    public Guid? DeleteUserId { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual CourseMaterial? CourseMaterialParent { get; set; }

    public virtual ICollection<CourseMaterial> InverseCourseMaterialParent { get; set; } = new List<CourseMaterial>();
}
