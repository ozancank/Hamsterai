namespace DataAccess.EF.Configuration;

public class RPackageSchoolConfiguration : IEntityTypeConfiguration<RPackageSchool>
{
    public void Configure(EntityTypeBuilder<RPackageSchool> builder)
    {
        builder.ToTable("RPackageSchools");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.SchoolId).HasColumnName("SchoolId").HasColumnOrder(6).IsRequired();
        builder.Property(e => e.PackageId).HasColumnName("PackageId").HasColumnOrder(7).IsRequired();
        builder.HasIndex(e => new { e.PackageId, e.SchoolId }).HasDatabaseName("IX_RPackageSchools_1").IsUnique();

        builder.HasOne(d => d.School).WithMany(p => p.RPackageSchools).HasForeignKey(d => d.SchoolId).HasPrincipalKey(x => x.Id);
        builder.HasOne(d => d.Package).WithMany(p => p.RPackageSchools).HasForeignKey(d => d.PackageId).HasPrincipalKey(x => x.Id);

        builder.HasData([
                new(Guid.Parse("a1a84a26-a7e4-4671-a979-d65fbbbedec0"),true,1,new DateTime(2000, 01, 01), 1, new DateTime(2000, 01, 01),1,5),
            ]);
    }
}