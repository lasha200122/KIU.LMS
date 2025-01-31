namespace KIU.LMS.Persistence.Database.Configs;

public class TopicConfiguration : EntityConfiguration<Topic>
{
    public override void Configure(EntityTypeBuilder<Topic> builder)
    {
        builder.ToTable(nameof(Topic));
        base.Configure(builder);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(x => x.Course)
            .WithMany(x => x.Topics)
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
