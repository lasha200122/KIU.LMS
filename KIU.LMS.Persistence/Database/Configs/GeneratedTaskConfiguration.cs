namespace KIU.LMS.Persistence.Database.Configs;

public class GeneratedTaskConfiguration : EntityConfiguration<GeneratedTask>
{
    public override void Configure(EntityTypeBuilder<GeneratedTask> builder)
    {
        builder.ToTable(nameof(GeneratedTask));
        base.Configure(builder);
        
        builder.Property(x => x.TaskDescription)
            .IsRequired()
            .HasMaxLength(2000);
        
        builder.Property(x => x.CodeSolution)
            .IsRequired()
            .HasMaxLength(2000);
        
        builder.Property(x => x.CodeGenerationPrompt)
            .IsRequired()
            .HasMaxLength(2000);
        
        builder.Property(x => x.CodeGradingPrompt)
            .IsRequired()
            .HasMaxLength(2000);
        
        builder.HasOne(x => x.Assignment)
            .WithMany(x => x.Tasks)
            .HasForeignKey(x => x.GeneratedAssignmentId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}