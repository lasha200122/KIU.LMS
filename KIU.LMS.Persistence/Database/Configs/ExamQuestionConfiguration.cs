namespace KIU.LMS.Persistence.Database.Configs;

public class ExamQuestionConfiguration : EntityConfiguration<ExamQuestion>
{
    public override void Configure(EntityTypeBuilder<ExamQuestion> builder)
    {
        builder.ToTable(nameof(ExamQuestion));
        base.Configure(builder);

        builder.Property(x => x.NumberOfQuestions)
            .IsRequired();

        builder.HasOne(x => x.Exam)
            .WithMany(x => x.Questions)
            .HasForeignKey(x => x.ExamId)
            .IsRequired();

        builder.HasOne(x => x.QuestionBank)
            .WithMany(x => x.ExamQuestions)
            .HasForeignKey(x => x.QuestionBankId)
            .IsRequired();
    }
}