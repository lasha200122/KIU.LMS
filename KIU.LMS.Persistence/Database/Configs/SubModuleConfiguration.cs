namespace KIU.LMS.Persistence.Database.Configs;

public class SubModuleConfiguration: EntityConfiguration<SubModule>
{
    public override void Configure(EntityTypeBuilder<SubModule> builder)
    {
        builder.ToTable(nameof(SubModule));
        base.Configure(builder);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(x => x.Module)
            .WithMany(x => x.SubModules)
            .HasForeignKey(x => x.ModuleId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
