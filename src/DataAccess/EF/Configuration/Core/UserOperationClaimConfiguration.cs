using Domain.Entities.Core;

namespace DataAccess.EF.Configuration.Core;

public class UserOperationClaimConfiguration : IEntityTypeConfiguration<UserOperationClaim>
{
    public void Configure(EntityTypeBuilder<UserOperationClaim> builder)
    {
        builder.ToTable("UserOperationClaims");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.UserId).HasColumnName("UserId").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.OperationClaimId).HasColumnName("OperationClaimId").HasColumnOrder(2).IsRequired();

        builder.HasOne(e => e.User).WithMany(e => e.UserOperationClaims).HasForeignKey(e => e.UserId).HasPrincipalKey(e => e.Id);
        builder.HasOne(e => e.OperationClaim).WithMany(e => e.UserOperationClaims).HasForeignKey(e => e.OperationClaimId).HasPrincipalKey(e => e.Id);

        var userOperationClaimSeed = new List<UserOperationClaim>
        {
            new(-1, 1, 1)
        };

        for (byte i = 1; i < OperationClaimContainer.OperationClaimList.Length; i++)
        {
            userOperationClaimSeed.Add(new UserOperationClaim(-i - 1, 2, OperationClaimContainer.OperationClaimList[i].Id));
        }

        builder.HasData(userOperationClaimSeed);
    }
}