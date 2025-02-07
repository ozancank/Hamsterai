using Domain.Entities.Core;

namespace DataAccess.EF.Configuration.Core;

public class OperationClaimConfiguration : IEntityTypeConfiguration<OperationClaim>
{
    public void Configure(EntityTypeBuilder<OperationClaim> builder)
    {
        builder.ToTable("OperationClaims");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasColumnName("Name").HasMaxLength(100).HasColumnOrder(1).IsRequired();
        builder.HasIndex(e => e.Name).HasDatabaseName("IX_OperationClaims_Name").IsUnique();
        builder.Property(e => e.Description).HasColumnName("Description").HasMaxLength(200).HasColumnOrder(2);
        builder.Property(e => e.ParentId).HasColumnName("ParentId").HasColumnOrder(3).HasDefaultValue(0).IsRequired();

        builder.HasData(OperationClaimContainer.OperationClaimList.ToArray());
    }
}