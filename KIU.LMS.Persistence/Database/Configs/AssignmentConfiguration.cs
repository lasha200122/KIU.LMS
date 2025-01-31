namespace KIU.LMS.Persistence.Database.Configs;

public class AssignmentConfiguration : EntityConfiguration<Assignment>
{
    public override void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.ToTable(nameof(Assignment));
        base.Configure(builder);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(x => x.Course)
            .WithMany(x => x.Assignments)
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Topic)
            .WithMany(x => x.Assignments)
            .HasForeignKey(x => x.TopicId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
