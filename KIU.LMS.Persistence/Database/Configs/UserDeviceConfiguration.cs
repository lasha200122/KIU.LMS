namespace KIU.LMS.Persistence.Database.Configs;

public class UserDeviceConfiguration : EntityConfiguration<UserDevice>
{
    public override void Configure(EntityTypeBuilder<UserDevice> builder)
    {
        builder.ToTable(nameof(UserDevice));
        base.Configure(builder);

        builder.Property(x => x.DeviceIdentifier)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Devices)
            .HasForeignKey(x => x.UserId)
            .IsRequired();
    }
}