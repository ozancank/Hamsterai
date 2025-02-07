namespace DataAccess.EF.Configuration;

public class PackageCategoryConfiguration : IEntityTypeConfiguration<PackageCategory>
{
    public void Configure(EntityTypeBuilder<PackageCategory> builder)
    {
        builder.ToTable("PackageCategories");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasMaxLength(50).HasColumnOrder(6).IsRequired();
        builder.Property(e => e.SortNo).HasColumnName("SortNo").HasDefaultValue(0).HasColumnOrder(7).IsRequired();
        builder.Property(e => e.IsWebVisible).HasColumnName("IsWebVisible").HasDefaultValue(false).HasColumnOrder(8).IsRequired();
        builder.Property(e => e.ParentId).HasColumnName("ParentId").HasDefaultValue(0).HasColumnOrder(9).IsRequired();
        builder.HasIndex(e => new { e.Name, e.ParentId }).IsUnique();
    }
}