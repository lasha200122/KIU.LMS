namespace KIU.LMS.Persistence.Database.Configs;

using Domain.Entities.SQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AssignmentSolutionJobConfiguration : EntityConfiguration<AssignmentSolutionJob>
{
    public override void Configure(EntityTypeBuilder<AssignmentSolutionJob> builder)
    {
        builder.ToTable(nameof(AssignmentSolutionJob));
        base.Configure(builder);

        builder.Property(x => x.AssignmentId)
            .IsRequired();

        builder.Property(x => x.SolutionId)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>() 
            .IsRequired();

        builder.Property(x => x.Attempts)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.Meta)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.Result)
            .IsRequired(false);

        builder.HasIndex(x => new { x.Status, x.Attempts });
        builder.HasIndex(x => x.AssignmentId);
        builder.HasIndex(x => x.SolutionId);
    }
}
