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
    }
}
