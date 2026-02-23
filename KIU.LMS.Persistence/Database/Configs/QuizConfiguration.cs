namespace KIU.LMS.Persistence.Database.Configs;

public class QuizConfiguration : EntityConfiguration<Quiz>
{
    public override void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.ToTable(nameof(Quiz));
        base.Configure(builder);

        builder.HasOne(x => x.Course)
            .WithMany(x => x.Quizzes)
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Topic)
            .WithMany(x => x.Quizzes)
            .HasForeignKey(x => x.TopicId)
            .OnDelete(DeleteBehavior.NoAction);

        // Safe Exam Browser Configuration
        builder.Property(x => x.RequiresSafeExamBrowser)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.SafeExamBrowserConfigKey)
            .HasMaxLength(256)
            .IsRequired(false);

        builder.Property(x => x.SafeExamBrowserConfigGeneratedAt)
            .IsRequired(false);
    }
}
