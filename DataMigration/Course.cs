using System;
using System.Collections.Generic;

namespace DataMigration;

public partial class Course
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

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual ICollection<CourseMaterial> CourseMaterials { get; set; } = new List<CourseMaterial>();

    public virtual ICollection<CourseMeeting> CourseMeetings { get; set; } = new List<CourseMeeting>();

    public virtual ICollection<Module> Modules { get; set; } = new List<Module>();

    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();

    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();

    public virtual ICollection<UserCourse> UserCourses { get; set; } = new List<UserCourse>();
}
