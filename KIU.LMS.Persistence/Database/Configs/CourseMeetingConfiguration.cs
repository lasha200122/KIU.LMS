namespace KIU.LMS.Persistence.Database.Configs;

public class CourseMeetingConfiguration : EntityConfiguration<CourseMeeting>
{
    public override void Configure(EntityTypeBuilder<CourseMeeting> builder)
    {
        builder.ToTable(nameof(CourseMeeting));
        base.Configure(builder);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.StartTime)
            .IsRequired();

        builder.Property(x => x.EndTime)
            .IsRequired();

        builder.HasOne(x => x.Course)
            .WithMany(x => x.Meetings)
            .HasForeignKey(x => x.CourseId)
            .IsRequired();
    }
}
