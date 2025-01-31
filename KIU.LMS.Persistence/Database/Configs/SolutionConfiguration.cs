namespace KIU.LMS.Persistence.Database.Configs;

public class SolutionConfiguration : EntityConfiguration<Solution>
{
    public override void Configure(EntityTypeBuilder<Solution> builder)
    {
        builder.ToTable(nameof(Solution));
        base.Configure(builder);

        builder.HasOne(x => x.Assignment)
            .WithMany(x => x.Solutions)
            .HasForeignKey(x => x.AssignmentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Solutions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
