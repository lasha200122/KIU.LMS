namespace KIU.LMS.Persistence.Database.Configs;

public class VotingOptionConfiguration : EntityConfiguration<VotingOption>
{
    public override void Configure(EntityTypeBuilder<VotingOption> builder)
    {
        base.Configure(builder);
        
        builder.HasIndex(v => new { v.SessionId, v.FileRecordId })
            .IsUnique();

        builder.HasOne(x => x.FileRecord) 
            .WithMany()
            .HasForeignKey(x => x.FileRecordId)
            .IsRequired(false); 
    }
}