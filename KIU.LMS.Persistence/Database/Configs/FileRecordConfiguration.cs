namespace KIU.LMS.Persistence.Database.Configs;

public class FileRecordConfiguration : EntityConfiguration<FileRecord>
{
    public override void Configure(EntityTypeBuilder<FileRecord> builder)
    {
        builder.ToTable(nameof(FileRecord));
        base.Configure(builder);
    }
}
