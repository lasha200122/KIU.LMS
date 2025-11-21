namespace KIU.LMS.Persistence.Database.Configs;

public class VotingConfiguration : EntityConfiguration<Vote>
{
    public override void Configure(EntityTypeBuilder<Vote> builder)
    {
        base.Configure(builder);
        
        builder.HasIndex(v => new { v.SessionId, v.UserId })
            .IsUnique();
        
        builder.HasOne<VotingSession>()
            .WithMany() 
            .HasForeignKey(v => v.SessionId);
    }
}