
namespace KIU.LMS.Persistence.Database.Configs;

public class ModuleConfiguration : EntityConfiguration<Domain.Entities.SQL.Module>
{
    public override void Configure(EntityTypeBuilder<Domain.Entities.SQL.Module> builder)
    {
        builder.ToTable(nameof(Domain.Entities.SQL.Module));
        base.Configure(builder);

        // Properties
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.CourseId)
            .IsRequired();

        // Relationships
        builder.HasOne(x => x.Course)
            .WithMany(x => x.Modules)
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.ModuleBanks)
            .WithOne(x => x.Module)
            .HasForeignKey(x => x.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the private backing field for QuestionBanks
        builder.HasMany<QuestionBank>("_questionBanks")
            .WithOne()
            .HasForeignKey("ModuleId")
            .OnDelete(DeleteBehavior.Cascade);

        // This allows EF to access the private field
        builder.Metadata
            .FindNavigation(nameof(Domain.Entities.SQL.Module.QuestionBanks))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}