namespace KIU.LMS.Persistence.Database.Configs;

public class VotingSessionConfiguration : EntityConfiguration<VotingSession>
{
    public override void Configure(EntityTypeBuilder<VotingSession> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(100);
        
        builder.HasMany(s => s.Options)
            .WithOne()
            .HasForeignKey(o => o.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}