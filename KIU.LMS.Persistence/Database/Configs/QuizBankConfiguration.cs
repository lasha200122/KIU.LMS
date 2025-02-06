namespace KIU.LMS.Persistence.Database.Configs;

public class QuizBankConfiguration : EntityConfiguration<QuizBank>
{
    public override void Configure(EntityTypeBuilder<QuizBank> builder)
    {
        builder.ToTable(nameof(QuizBank));
        base.Configure(builder);

        builder.HasOne(x => x.Quiz)
            .WithMany(x => x.QuizBanks)
            .HasForeignKey(x => x.QuizId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.QuestionBank)
            .WithMany(x => x.QuizBanks)
            .HasForeignKey(x => x.QuestionBankId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
