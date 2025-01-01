namespace KIU.LMS.Persistence.Database.Configs;

public class UserConfiguration : EntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User));
        base.Configure(builder);

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Role)
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.PasswordSalt)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.EmailVerified)
            .IsRequired();

        builder.HasMany(x => x.UserCourses)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);

        builder.HasMany(x => x.LoginAttempts)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);

        builder.HasMany(x => x.Devices)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
    }
}