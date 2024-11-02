namespace DataAccess.EF.Configuration;

public class PackageConfiguration : IEntityTypeConfiguration<Package>
{
    public void Configure(EntityTypeBuilder<Package> builder)
    {
        builder.ToTable("Packages");
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
        builder.Property(e => e.UnitPrice).HasColumnName("UnitPrice").HasDefaultValue(0).HasColumnOrder(9).IsRequired();
        builder.Property(e => e.UnitOldPrice).HasColumnName("UnitOldPrice").HasColumnOrder(10);
        builder.Property(e => e.TaxRatio).HasColumnName("TaxRatio").HasDefaultValue(0).HasColumnOrder(11).IsRequired();
        builder.Property(e => e.TaxAmount).HasColumnName("TaxAmount").HasDefaultValue(0).HasColumnOrder(12).IsRequired();
        builder.Property(e => e.TaxOldAmount).HasColumnName("TaxOldAmount").HasColumnOrder(13);
        builder.Property(e => e.Amount).HasColumnName("Amount").HasDefaultValue(0).HasColumnOrder(14).IsRequired();
        builder.Property(e => e.OldAmount).HasColumnName("OldAmount").HasColumnOrder(15);
        builder.Property(e => e.PaymentRenewalPeriod).HasColumnName("PaymentRenewalPeriod").HasDefaultValue(PaymentRenewalPeriod.None).HasColumnOrder(16).IsRequired();
        builder.Property(e => e.Description).HasColumnName("Description").HasMaxLength(500).HasColumnOrder(17);
        builder.Property(e => e.PictureUrl).HasColumnName("PictureUrl").HasMaxLength(500).HasColumnOrder(18);
        builder.Property(e => e.Slug).HasColumnName("Slug").HasMaxLength(50).HasColumnOrder(19);
        builder.Property(e => e.CategoryId).HasColumnName("CategoryId").HasColumnOrder(20);

        builder.HasIndex(e => new { e.Name, e.PaymentRenewalPeriod }).HasDatabaseName("IX_Packages_1").IsUnique();

        builder.HasOne(d => d.PackageCategory).WithMany(p => p.Packages).HasForeignKey(d => d.CategoryId).HasPrincipalKey(x => x.Id);

        builder.HasData([
                new(1, true, 1, new DateTime(2000, 01, 01), 1, new DateTime(2000, 01, 01), "Sözel"),
                new(2, true, 1, new DateTime(2000, 01, 01), 1, new DateTime(2000, 01, 01), "Sayısal"),
                new(3, true, 1, new DateTime(2000, 01, 01), 1, new DateTime(2000, 01, 01), "Eşit Ağırlık"),
                new(4, true, 1, new DateTime(2000, 01, 01), 1, new DateTime(2000, 01, 01), "Yabancı Dil"),
                new(5, true, 1, new DateTime(2000, 01, 01), 1, new DateTime(2000, 01, 01), "Hepsi"),
                new(6, true, 1, new DateTime(2000, 01, 01), 1, new DateTime(2000, 01, 01), "Orta Okul"),
                new(7, true, 1, new DateTime(2000, 01, 01), 1, new DateTime(2000, 01, 01), "Lise"),
            ]);
    }
}