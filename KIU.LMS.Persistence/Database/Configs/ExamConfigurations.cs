namespace KIU.LMS.Persistence.Database.Configs;

public class ExamConfigurations : EntityConfiguration<Exam>
{
    public override void Configure(EntityTypeBuilder<Exam> builder)
    {
        builder.ToTable(nameof(Exam));
        base.Configure(builder);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.StartTime)
            .IsRequired();

        builder.Property(x => x.EndTime)
            .IsRequired();

        builder.Property(x => x.DurationInMinutes)
            .IsRequired();

        builder.Property(x => x.MaxScore)
            .IsRequired();

        builder.HasOne(x => x.Course)
            .WithMany(x => x.Exams)
            .HasForeignKey(x => x.CourseId)
            .IsRequired();

        builder.HasMany(x => x.Questions)
            .WithOne(x => x.Exam)
            .HasForeignKey(x => x.ExamId);

        builder.HasMany(x => x.Attempts)
            .WithOne(x => x.Exam)
            .HasForeignKey(x => x.ExamId);
    }
}
