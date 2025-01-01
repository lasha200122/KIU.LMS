namespace KIU.LMS.Domain.Entities.SQL;

public class UserCourse : Aggregate
{
    public Guid UserId { get; private set; }
    public Guid CourseId { get; private set; }
    public DateTimeOffset CanAccessTill { get; private set; }

    public virtual User User { get; private set; } = null!;
    public virtual Course Course { get; private set; } = null!;

    public UserCourse() { }

    public UserCourse(
        Guid id,
        Guid userId,
        Guid courseId,
        DateTimeOffset canAccessTill,
        Guid createUserId) : base(id, DateTimeOffset.Now, createUserId)
    {
        UserId = userId;
        CourseId = courseId;
        CanAccessTill = canAccessTill;
        Validate(this);
    }

    private void Validate(UserCourse userCourse)
    {
        if (userCourse.UserId == default)
            throw new Exception("მომხმარებლის ID სავალდებულოა");
        if (userCourse.CourseId == default)
            throw new Exception("კურსის ID სავალდებულოა");
    }
}