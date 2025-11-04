namespace KIU.LMS.Persistence.Database.Configs;

public class GeneratedQuestionConfiguration : EntityConfiguration<GeneratedQuestion>
{
    public override void Configure(EntityTypeBuilder<GeneratedQuestion> builder)
    {
        builder.ToTable(nameof(GeneratedQuestion));
        base.Configure(builder);

        builder.Property(x => x.QuestionText)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.OptionA)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.OptionB)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.OptionC)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.OptionD)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.ExplanationCorrect)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.ExplanationIncorrect)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasOne(x => x.Assignment)
            .WithMany(x => x.Questions)
            .HasForeignKey(x => x.GeneratedAssignmentId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}