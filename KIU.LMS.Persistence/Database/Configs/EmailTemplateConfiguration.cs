namespace KIU.LMS.Persistence.Database.Configs;

public class EmailTemplateConfiguration : EntityConfiguration<EmailTemplate>
{
    public override void Configure(EntityTypeBuilder<EmailTemplate> builder)
    {
        builder.ToTable(nameof(EmailTemplate));
        base.Configure(builder);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Body)
            .IsRequired();

        builder.Property(x => x.Variables)
            .IsRequired();

        builder.Property(x => x.Subject)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasMany(x => x.EmailQueue)
            .WithOne(x => x.Template)
            .HasForeignKey(x => x.TemplateId);
    }
}
