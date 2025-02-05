namespace KIU.LMS.Persistence.Database.Configs;

public class PromptConfiguration : EntityConfiguration<Prompt>
{
    public override void Configure(EntityTypeBuilder<Prompt> builder)
    {
        builder.ToTable(nameof(Prompt));
        base.Configure(builder);

        builder.HasMany(x => x.Assignments)
            .WithOne(x => x.Prompt)
            .HasForeignKey(x => x.PromptId);
    }
}