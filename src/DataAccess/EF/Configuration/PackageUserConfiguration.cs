using OCK.Core.Utilities.Numbers;

namespace DataAccess.EF.Configuration;

public class PackageUserConfiguration : IEntityTypeConfiguration<PackageUser>
{
    public void Configure(EntityTypeBuilder<PackageUser> builder)
    {
        builder.ToTable("PackageUsers");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.PackageId).HasColumnName("PackageId").HasColumnOrder(6).IsRequired();
        builder.Property(e => e.UserId).HasColumnName("UserId").HasColumnOrder(7).IsRequired();
        builder.Property(e => e.RenewCount).HasColumnName("RenewCount").HasDefaultValue(0).HasColumnOrder(8).IsRequired();
        builder.Property(e => e.EndDate).HasColumnName("EndDate").HasDefaultValue(new DateTime(2024, 12, 23)).HasColumnOrder(9).IsRequired();
        builder.Property(e => e.QuestionCredit).HasColumnName("QuestionCredit").HasDefaultValue(0).HasColumnOrder(10).IsRequired();

        builder.HasIndex(e => new { e.PackageId, e.UserId }).HasDatabaseName("IX_RPackageUsers_1").IsUnique();

        builder.HasOne(d => d.Package).WithMany(p => p.PackageUsers).HasForeignKey(d => d.PackageId).HasPrincipalKey(x => x.Id);
        builder.HasOne(d => d.User).WithMany(p => p.PackageUsers).HasForeignKey(d => d.UserId).HasPrincipalKey(x => x.Id);

        var seeds = new List<PackageUser>();
        for (int i = 1; i <= 20; i++)
        {
            if (i is 16 or 17) continue;
            seeds.Add(new(NumberTools.IntToGuid(i), true, 1, new DateTime(2000, 01, 01), 1, new DateTime(2000, 01, 01), 5, i, new DateTime(9999, 1, 1), 9999));
        }

        builder.HasData(seeds);
    }
}