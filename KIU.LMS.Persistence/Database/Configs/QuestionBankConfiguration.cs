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

        builder.HasOne(x => x.Module)
            .WithMany(x => x.QuestionBanks)
            .HasForeignKey(x => x.ModuleId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}