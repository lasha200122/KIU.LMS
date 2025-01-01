namespace KIU.LMS.Persistence.Database.Configs;

public class ExamAttemptConfiguration : EntityConfiguration<ExamAttempt>
{
    public override void Configure(EntityTypeBuilder<ExamAttempt> builder)
    {
        builder.ToTable(nameof(ExamAttempt));
        base.Configure(builder);

        builder.Property(x => x.AttemptNumber)
            .IsRequired();

        builder.Property(x => x.StartedAt)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.Exam)
            .WithMany(x => x.Attempts)
            .HasForeignKey(x => x.ExamId)
            .IsRequired();
    }
}