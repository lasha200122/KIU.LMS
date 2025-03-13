
namespace KIU.LMS.Persistence.Database.Configs;

public class ModuleConfiguration : EntityConfiguration<Domain.Entities.SQL.Module>
{
    public override void Configure(EntityTypeBuilder<Domain.Entities.SQL.Module> builder)
    {
        builder.ToTable(nameof(Domain.Entities.SQL.Module));
        base.Configure(builder);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(x => x.Course)
            .WithMany(x => x.Modules)
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
