namespace DataAccess.EF.Configuration;

public class SchoolConfiguration : IEntityTypeConfiguration<School>
{
    public void Configure(EntityTypeBuilder<School> builder)
    {
        builder.ToTable("Schools");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasMaxLength(250).HasColumnOrder(6).IsRequired();
        builder.HasIndex(e => e.Name).HasDatabaseName("UK_Schools_Name").IsUnique();
        builder.Property(e => e.TaxNumber).HasColumnName("TaxNumber").HasMaxLength(11).HasColumnOrder(7).IsRequired();
        builder.HasIndex(e => e.TaxNumber).HasDatabaseName("UK_Schools_TaxNumber").IsUnique();
        builder.Property(e => e.Address).HasColumnName("Address").HasColumnOrder(8).IsRequired();
        builder.Property(e => e.City).HasColumnName("City").HasMaxLength(50).HasColumnOrder(9).IsRequired();
        builder.Property(e => e.AuthorizedName).HasColumnName("AuthorizedName").HasMaxLength(100).HasColumnOrder(10).IsRequired();
        builder.Property(e => e.AuthorizedPhone).HasColumnName("AuthorizedPhone").HasMaxLength(15).HasColumnOrder(11).IsRequired();
        builder.Property(e => e.AuthorizedEmail).HasColumnName("AuthorizedEmail").HasMaxLength(100).HasColumnOrder(12).IsRequired();
        builder.Property(e => e.UserCount).HasColumnName("UserCount").HasColumnOrder(13).IsRequired();
        builder.Property(e => e.LicenseEndDate).HasColumnName("LicenseEndDate").HasColumnOrder(14).IsRequired();
        builder.Property(e => e.AccessStundents).HasColumnName("AccessStundents").HasDefaultValue(true).HasColumnOrder(15).IsRequired();

        builder.HasData(
        [
            new(1, true, 2, new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01))
            {
                Name= "Hamster Koleji",
                City="Gaziantep",
                TaxNumber="9999999999",
                Address= "TeknoPark",
                AuthorizedName="Yetkili",
                AuthorizedPhone="5999999999",
                AuthorizedEmail="okul@mail.com",
                UserCount=157,
                LicenseEndDate=new DateTime(2050, 01, 01),
            }
        ]);
    }
}