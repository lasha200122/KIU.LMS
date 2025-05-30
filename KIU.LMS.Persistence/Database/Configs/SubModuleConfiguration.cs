namespace KIU.LMS.Persistence.Database.Configs;

public class SubModuleConfiguration : EntityConfiguration<SubModule>
{
    public override void Configure(EntityTypeBuilder<SubModule> builder)
    {
        builder.ToTable(nameof(SubModule));
        base.Configure(builder);

        // Properties
        builder.Property(x => x.TaskDescription)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.CodeSolution)
            .HasMaxLength(4000)
            .IsRequired(false);

        builder.Property(x => x.CodeGenerationPrompt)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.CodeGraidingPrompt)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.Solution)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.Difficulty)
            .HasConversion<int>()
            .IsRequired(false);

        // Foreign key
        builder.Property(x => x.ModuleBankId)
            .IsRequired();

        // Relationships
        builder.HasOne(x => x.ModuleBank)
            .WithMany(x => x.SubModules)
            .HasForeignKey(x => x.ModuleBankId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ModuleBankConfiguration : EntityConfiguration<ModuleBank>
{
    public override void Configure(EntityTypeBuilder<ModuleBank> builder)
    {
        builder.ToTable(nameof(ModuleBank));
        base.Configure(builder);

        // Properties
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.ModuleId)
            .IsRequired();

        // Relationships
        builder.HasOne(x => x.Module)
            .WithMany(x => x.ModuleBanks)
            .HasForeignKey(x => x.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.SubModules)
            .WithOne(x => x.ModuleBank)
            .HasForeignKey(x => x.ModuleBankId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}