namespace KIU.LMS.Persistence.Database.Configs.Base;

public class EntityConfiguration<T> : IEntityTypeConfiguration<T> where T : Aggregate
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(x => x.Id);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreateDate)
            .IsRequired();

        builder.HasIndex(x => x.CreateDate);

        builder.Property(x => x.LastUpdateDate)
            .IsRequired(false);

        builder.Property(x => x.DeleteDate)
            .IsRequired(false);

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
