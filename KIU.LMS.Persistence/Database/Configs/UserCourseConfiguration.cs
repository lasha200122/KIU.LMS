namespace KIU.LMS.Persistence.Database.Configs;

public class UserCourseConfiguration : EntityConfiguration<UserCourse>
{
    public override void Configure(EntityTypeBuilder<UserCourse> builder)
    {
        builder.ToTable(nameof(UserCourse));
        base.Configure(builder);

        builder.Property(x => x.CanAccessTill)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserCourses)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.Course)
            .WithMany(x => x.UserCourses)
            .HasForeignKey(x => x.CourseId)
            .IsRequired();
    }
}