using KIU.LMS.Domain.Entities.SQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KIU.LMS.Persistence.Database.Configs;

public class GeneratedAssignmentConfiguration : EntityConfiguration<GeneratedAssignment>
{
    public override void Configure(EntityTypeBuilder<GeneratedAssignment> builder)
    {
        builder.ToTable(nameof(GeneratedAssignment));
        base.Configure(builder);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasMany(x => x.Questions)
            .WithOne(x => x.Assignment)
            .HasForeignKey(x => x.GeneratedAssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Tasks)
            .WithOne(x => x.Assignment)
            .HasForeignKey(x => x.GeneratedAssignmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}