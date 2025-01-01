namespace KIU.LMS.Persistence.Database.Configs;

public class CourseConfiguration : EntityConfiguration<Course>
{
    public override void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable(nameof(Course));
        base.Configure(builder);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasMany(x => x.UserCourses)
            .WithOne(x => x.Course)
            .HasForeignKey(x => x.CourseId);

        builder.HasMany(x => x.Materials)
            .WithOne(x => x.Course)
            .HasForeignKey(x => x.CourseId);

        builder.HasMany(x => x.Meetings)
            .WithOne(x => x.Course)
            .HasForeignKey(x => x.CourseId);

        builder.HasMany(x => x.Exams)
            .WithOne(x => x.Course)
            .HasForeignKey(x => x.CourseId);
    }
}
