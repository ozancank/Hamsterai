namespace DataAccess.EF.Configuration;

public class RPackageLessonConfiguration : IEntityTypeConfiguration<RPackageLesson>
{
    public void Configure(EntityTypeBuilder<RPackageLesson> builder)
    {
        builder.ToTable("RPackageLessons");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.PackageId).HasColumnName("PackageId").HasColumnOrder(6).IsRequired();
        builder.Property(e => e.LessonId).HasColumnName("LessonId").HasColumnOrder(7).IsRequired();
        builder.HasIndex(e => new { e.PackageId, e.LessonId }).HasDatabaseName("IX_RPackageLesson_1").IsUnique();

        builder.HasOne(d => d.Package).WithMany(p => p.RPackageLessons).HasForeignKey(d => d.PackageId).HasPrincipalKey(x => x.Id);
        builder.HasOne(d => d.Lesson).WithMany(p => p.RPackageLessons).HasForeignKey(d => d.LessonId).HasPrincipalKey(x => x.Id);

        builder.HasData([
                new(Guid.Parse("a1a84a26-a7e4-4671-a979-d65fbbbedec0"),true,2,new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01),1,1),
                new(Guid.Parse("a1a84a26-a7e4-4671-a979-d65fbbbedec1"),true,2,new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01),2,2),
                new(Guid.Parse("a1a84a26-a7e4-4671-a979-d65fbbbedec2"),true,2,new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01),1,3),
                new(Guid.Parse("a1a84a26-a7e4-4671-a979-d65fbbbedec3"),true,2,new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01),2,4),
                new(Guid.Parse("a1a84a26-a7e4-4671-a979-d65fbbbedec4"),true,2,new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01),2,5),
                new(Guid.Parse("a1a84a26-a7e4-4671-a979-d65fbbbedec5"),true,2,new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01),2,6),
                new(Guid.Parse("a1a84a26-a7e4-4671-a979-d65fbbbedec6"),true,2,new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01),2,7),
                new(Guid.Parse("a1a84a26-a7e4-4671-a979-d65fbbbedec7"),true,2,new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01),1,8),
                new(Guid.Parse("a1a84a26-a7e4-4671-a979-d65fbbbedec8"),true,2,new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01),1,9),
                new(Guid.Parse("a1a84a26-a7e4-4671-a979-d65fbbbedec9"),true,2,new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01),1,10),
                new(Guid.Parse("a1a84a26-a7e4-4671-a979-d65fbbbedeca"),true,2,new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01),1,11),
                new(Guid.Parse("a1a84a26-a7e4-4671-a979-d65fbbbedecb"),true,2,new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01),1,12),
            ]);
    }
}