namespace KIU.LMS.Persistence.Database.Configs;

public class LoginAttemptConfiguration : EntityConfiguration<LoginAttempt>
{
    public override void Configure(EntityTypeBuilder<LoginAttempt> builder)
    {
        builder.ToTable(nameof(LoginAttempt));
        base.Configure(builder);

        builder.Property(x => x.IpAddress)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.UserAgent)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.DeviceIdentifier)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(x => x.User)
            .WithMany(x => x.LoginAttempts)
            .HasForeignKey(x => x.UserId)
            .IsRequired();
    }
}
