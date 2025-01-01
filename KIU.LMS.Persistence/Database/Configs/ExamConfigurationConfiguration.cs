namespace KIU.LMS.Persistence.Database.Configs;

public class ExamConfigurationConfiguration : EntityConfiguration<ExamConfiguration>
{
    public override void Configure(EntityTypeBuilder<ExamConfiguration> builder)
    {
        builder.ToTable(nameof(ExamConfiguration));
        base.Configure(builder);

        builder.Property(x => x.Attempts)
            .IsRequired();

        builder.Property(x => x.LateMinutes)
            .IsRequired();

        builder.Property(x => x.ReconnectAttempts)
            .IsRequired();

        builder.Property(x => x.ReconnectMinutes)
            .IsRequired();

        builder.Property(x => x.EnableIpRestriction)
            .IsRequired();

        builder.Property(x => x.AllowedIpRanges)
            .IsRequired();

        builder.HasOne(x => x.Exam)
            .WithOne()
            .HasForeignKey<ExamConfiguration>(x => x.ExamId)
            .IsRequired();
    }
}