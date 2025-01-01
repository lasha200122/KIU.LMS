namespace KIU.LMS.Persistence.Database.Configs;

public class QuestionBankConfiguration : EntityConfiguration<QuestionBank>
{
    public override void Configure(EntityTypeBuilder<QuestionBank> builder)
    {
        builder.ToTable(nameof(QuestionBank));
        base.Configure(builder);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasMany(x => x.ExamQuestions)
            .WithOne(x => x.QuestionBank)
            .HasForeignKey(x => x.QuestionBankId);
    }
}