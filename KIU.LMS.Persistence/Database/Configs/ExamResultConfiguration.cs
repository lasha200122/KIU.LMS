namespace KIU.LMS.Persistence.Database.Configs;

public class ExamResultConfiguration : EntityConfiguration<ExamResult>
{
    public override void Configure(EntityTypeBuilder<ExamResult> builder)
    {
        builder.ToTable(nameof(ExamResult));
        base.Configure(builder);

        builder.HasOne(x => x.User)
            .WithMany(x => x.ExamResults)
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Quiz)
            .WithMany(x => x.ExamResults)
            .HasForeignKey(x => x.QuizId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
