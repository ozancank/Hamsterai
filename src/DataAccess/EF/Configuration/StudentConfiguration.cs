namespace DataAccess.EF.Configuration;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasMaxLength(50).HasColumnOrder(6).IsRequired();
        builder.Property(e => e.Surname).HasColumnName("Surname").HasMaxLength(50).HasColumnOrder(7).IsRequired();
        builder.Property(e => e.StudentNo).HasColumnName("StudentNo").HasMaxLength(15).HasColumnOrder(8).IsRequired();
        builder.Property(e => e.Phone).HasColumnName("Phone").HasMaxLength(15).HasColumnOrder(10);
        //builder.HasIndex(e => e.Phone).HasDatabaseName("UK_Students_Phone").IsUnique();
        builder.Property(e => e.Email).HasColumnName("Email").HasMaxLength(100).HasColumnOrder(11).IsRequired();
        builder.HasIndex(e => e.Email).HasDatabaseName("UK_Students_Email").IsUnique();
        builder.Property(e => e.ClassRoomId).HasColumnName("ClassRoomId").HasColumnOrder(12).IsRequired();

        builder.HasOne(d => d.ClassRoom).WithMany(p => p.Students).HasForeignKey(d => d.ClassRoomId).HasPrincipalKey(x => x.Id);

        builder.HasData(
        [
            new Student(1, true, 2, new DateTime(2000, 1, 1), 2, new DateTime(2000, 1, 1))
            {
                Name = "Öğrenci 1",
                Surname = "Kullanıcı",
                StudentNo = "001",
                Phone = "5000000006",
                Email = "ogrenci1@mail.com",
                ClassRoomId = 1,
            },
            new Student(2, true, 2, new DateTime(2000, 1, 1), 2, new DateTime(2000, 1, 1))
            {
                Name = "Öğrenci 2",
                Surname = "Kullanıcı",
                StudentNo = "002",
                Phone = "5000000007",
                Email = "ogrenci2@mail.com",
                ClassRoomId = 1,
            },
            new Student(3, true, 2, new DateTime(2000, 1, 1), 2, new DateTime(2000, 1, 1))
            {
                Name = "Öğrenci 3",
                Surname = "Kullanıcı",
                StudentNo = "003",
                Phone = "5000000008",
                Email = "ogrenci3@mail.com",
                ClassRoomId = 2,
            },
            new Student(4, true, 2, new DateTime(2000, 1, 1), 2, new DateTime(2000, 1, 1))
            {
                Name = "Öğrenci 4",
                Surname = "Kullanıcı",
                StudentNo = "004",
                Phone = "5000000009",
                Email = "ogrenci4@mail.com",
                ClassRoomId = 2,
            },
            new Student(5, true, 2, new DateTime(2000, 1, 1), 2, new DateTime(2000, 1, 1))
            {
                Name = "Öğrenci 5",
                Surname = "Kullanıcı",
                StudentNo = "005",
                Phone = "5000000010",
                Email = "ogrenci5@mail.com",
                ClassRoomId = 3,
            },
            new Student(6, true, 2, new DateTime(2000, 1, 1), 2, new DateTime(2000, 1, 1))
            {
                Name = "Öğrenci 6",
                Surname = "Kullanıcı",
                StudentNo = "006",
                Phone = "5000000011",
                Email = "ogrenci6@mail.com",
                ClassRoomId = 3,
            },
            new Student(7, true, 2, new DateTime(2000, 1, 1), 2, new DateTime(2000, 1, 1))
            {
                Name = "Ozan Can",
                Surname = "Kösemez",
                StudentNo = "007",
                Phone = "5069151010",
                Email = "ozancank@gmail.com",
                ClassRoomId = 1,
            },
            new Student(8, true, 2, new DateTime(2000, 1, 1), 2, new DateTime(2000, 1, 1))
            {
                Name = "Alican",
                Surname = "Kesen",
                StudentNo = "008",
                Phone = "5313914388",
                Email = "942alicankesen@gmail.com",
                ClassRoomId = 1,
            },
            new Student(9, true, 2, new DateTime(2000, 1, 1), 2, new DateTime(2000, 1, 1))
            {
                Name = "Eyüp",
                Surname = "Balcan",
                StudentNo = "009",
                Phone = "5550593005",
                Email = "balcan1905@gmail.com",
                ClassRoomId = 1,
            }
        ]);
    }
}