namespace KIU.LMS.Persistence.Database.Configs;

public class EmailQueueConfiguration : EntityConfiguration<EmailQueue>
{
    public override void Configure(EntityTypeBuilder<EmailQueue> builder)
    {
        builder.ToTable(nameof(EmailQueue));
        base.Configure(builder);

        builder.Property(x => x.ToEmail)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Variables)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.FailureReason)
            .HasMaxLength(500);

        builder.Property(x => x.RetryCount)
            .IsRequired();

        builder.HasOne(x => x.Template)
            .WithMany(x => x.EmailQueue)
            .HasForeignKey(x => x.TemplateId)
            .IsRequired();
    }
}