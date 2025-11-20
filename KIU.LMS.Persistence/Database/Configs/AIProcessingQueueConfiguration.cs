namespace KIU.LMS.Persistence.Database.Configs;

public class AIProcessingQueueConfiguration : EntityConfiguration<AIProcessingQueue>
{
    public override void Configure(EntityTypeBuilder<AIProcessingQueue> builder)
    {
        builder.ToTable(nameof(AIProcessingQueue));
        base.Configure(builder);
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.TargetId)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>() 
            .IsRequired();

        builder.Property(x => x.MetaData)
            .HasColumnType("text")
            .IsRequired(false);

        builder.HasIndex(x => new { x.Status, x.Type })
            .HasDatabaseName("IX_AIQueue_Status_Type");
    }
}