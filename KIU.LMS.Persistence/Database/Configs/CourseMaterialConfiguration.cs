namespace KIU.LMS.Persistence.Database.Configs;

public class CourseMaterialConfiguration : EntityConfiguration<CourseMaterial>
{
    public override void Configure(EntityTypeBuilder<CourseMaterial> builder)
    {
        builder.ToTable(nameof(CourseMaterial));
        base.Configure(builder);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Order)
            .IsRequired();

        builder.HasOne(x => x.Course)
            .WithMany(x => x.Materials)
            .HasForeignKey(x => x.CourseId)
            .IsRequired();

        builder.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.CourseMaterialParentId)
            .IsRequired(false);
    }
}