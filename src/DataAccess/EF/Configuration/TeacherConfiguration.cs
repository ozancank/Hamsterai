namespace DataAccess.EF.Configuration;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.ToTable("Teachers");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasMaxLength(100).HasColumnOrder(6).IsRequired();
        builder.Property(e => e.Surname).HasColumnName("Surname").HasMaxLength(100).HasColumnOrder(7).IsRequired();
        builder.Property(e => e.TcNo).HasColumnName("TcNo").HasMaxLength(11).HasColumnOrder(8);
        builder.HasIndex(e => e.TcNo).HasDatabaseName("UK_Teachers_TcNo").IsUnique();
        builder.Property(e => e.Phone).HasColumnName("Phone").HasMaxLength(15).HasColumnOrder(9).IsRequired();
        builder.HasIndex(e => e.Phone).HasDatabaseName("UK_Teachers_Phone").IsUnique();
        builder.Property(e => e.Email).HasColumnName("Email").HasMaxLength(100).HasColumnOrder(10).IsRequired();
        builder.HasIndex(e => e.Email).HasDatabaseName("UK_Teachers_Email").IsUnique();
        builder.Property(e => e.Branch).HasColumnName("Branch").HasMaxLength(50).HasColumnOrder(11).IsRequired();
        builder.Property(e => e.SchoolId).HasColumnName("SchoolId").HasColumnOrder(12).IsRequired();

        builder.HasOne(d => d.School).WithMany(p => p.Teachers).HasForeignKey(d => d.SchoolId).HasPrincipalKey(x => x.Id);

        builder.HasData(
        [
            new(1, true, 2, new DateTime(2000, 1, 1), 2, new DateTime(2000,1,1))
            {
                Name="Öğretmen 1",
                Surname="Kullanıcı",
                TcNo="11111111111",
                Phone="5000000004",
                Email="hoca1@mail.com",
                Branch="Matematik",
                SchoolId=1,
            },
            new(2, true, 2, new DateTime(2000, 1, 1), 2, new DateTime(2000,1,1))
            {
                Name="Öğretmen 2",
                Surname="Kullanıcı",
                TcNo="22222222222",
                Phone="5000000005",
                Email="hoca2@mail.com",
                Branch="Türkçe",
                SchoolId=1,
            },
        ]);
    }
}