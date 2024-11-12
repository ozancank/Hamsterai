using Domain.Entities.Core;

namespace DataAccess.EF.Configuration.Core;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.UserName).HasColumnName("UserName").HasMaxLength(250).HasColumnOrder(3).IsRequired();
        builder.HasIndex(e => e.UserName).HasDatabaseName("UK_Users_UserName").IsUnique();
        builder.Property(e => e.PasswordSalt).HasColumnName("PasswordSalt").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.PasswordHash).HasColumnName("PasswordHash").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.MustPasswordChange).HasColumnName("MustPasswordChange").HasColumnOrder(6).IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasMaxLength(250).HasColumnOrder(7).IsRequired();
        builder.Property(e => e.Surname).HasColumnName("Surname").HasMaxLength(250).HasColumnOrder(8).IsRequired();
        builder.Property(e => e.Phone).HasColumnName("Phone").HasMaxLength(15).HasColumnOrder(9);
        //builder.HasIndex(e => e.Phone).HasDatabaseName("UK_Users_Phone").IsUnique();
        builder.Property(e => e.ProfileUrl).HasColumnName("ProfileUrl").HasColumnOrder(10);
        builder.Property(e => e.Email).HasColumnName("Email").HasMaxLength(100).HasColumnOrder(11).IsRequired();
        builder.HasIndex(e => e.Email).HasDatabaseName("UK_Users_Email").IsUnique();
        builder.Property(e => e.Type).HasColumnName("Type").HasColumnOrder(12).IsRequired();
        builder.Property(e => e.SchoolId).HasColumnName("SchoolId").HasColumnOrder(13);
        builder.Property(e => e.ConnectionId).HasColumnName("ConnectionId").HasColumnOrder(14);
        builder.Property(e => e.PackageCredit).HasColumnName("PackageCredit").HasDefaultValue(0).HasColumnOrder(16).IsRequired();
        builder.Property(e => e.AddtionalCredit).HasColumnName("AddtionalCredit").HasDefaultValue(0).HasColumnOrder(17).IsRequired();
        builder.Property(e => e.AutomaticPayment).HasColumnName("AutomaticPayment").HasDefaultValue(false).HasColumnOrder(18).IsRequired();
        builder.Property(e => e.TaxNumber).HasColumnName("TaxNumber").HasMaxLength(11).HasColumnOrder(19);
        builder.Property(e => e.LicenceEndDate).HasColumnName("LicenceEndDate").HasDefaultValue(new DateTime(2000, 01, 01)).HasColumnOrder(20);

        builder.HasOne(e => e.School).WithMany(e => e.Users).HasForeignKey(e => e.SchoolId).HasPrincipalKey(e => e.Id);

        var passwordSalt1 = new byte[] { 22, 25, 49, 68, 114, 216, 25, 253, 239, 196, 230, 130, 40, 214, 153, 94, 28, 188, 154, 225, 50, 31, 161, 21, 4, 230, 179, 118, 232, 155, 171, 114, 197, 6, 252, 53, 35, 172, 165, 92, 20, 162, 101, 242, 248, 163, 238, 160, 154, 196, 49, 79, 75, 39, 86, 23, 235, 103, 53, 30, 125, 117, 85, 109, 131, 66, 2, 219, 134, 223, 230, 64, 180, 36, 225, 254, 237, 167, 255, 137, 54, 86, 113, 27, 104, 47, 172, 200, 53, 23, 217, 143, 228, 57, 211, 92, 242, 99, 140, 90, 93, 1, 134, 181, 53, 38, 226, 125, 45, 80, 44, 8, 43, 67, 20, 84, 161, 155, 150, 7, 31, 182, 239, 204, 76, 162, 82, 81 };
        var passwordHash1 = new byte[] { 169, 145, 33, 161, 147, 58, 15, 169, 19, 31, 236, 3, 128, 147, 151, 45, 188, 253, 68, 70, 153, 152, 73, 88, 253, 225, 151, 194, 216, 163, 110, 253, 172, 109, 2, 180, 132, 19, 48, 181, 217, 89, 227, 138, 159, 251, 96, 176, 113, 54, 11, 219, 157, 136, 251, 120, 124, 56, 153, 29, 36, 35, 129, 141 };

        //Sifre123+
        var passwordSalt2 = new byte[] { 110, 138, 57, 138, 8, 214, 100, 204, 96, 244, 112, 122, 216, 29, 143, 233, 207, 44, 246, 94, 145, 242, 43, 105, 129, 206, 65, 48, 233, 219, 35, 237, 138, 38, 46, 252, 49, 89, 130, 30, 31, 164, 44, 32, 185, 212, 83, 225, 98, 112, 163, 142, 69, 255, 194, 130, 80, 230, 18, 42, 105, 158, 161, 163, 212, 99, 63, 48, 166, 190, 0, 193, 209, 227, 88, 214, 227, 127, 237, 209, 34, 245, 113, 202, 224, 237, 193, 49, 143, 88, 2, 63, 145, 186, 148, 230, 187, 10, 74, 170, 207, 173, 100, 18, 117, 202, 224, 138, 24, 82, 148, 101, 188, 135, 109, 153, 7, 7, 30, 140, 252, 99, 195, 195, 20, 24, 253, 151 };
        var passwordHash2 = new byte[] { 91, 194, 62, 143, 170, 150, 7, 228, 50, 239, 166, 107, 207, 24, 2, 7, 203, 74, 96, 38, 172, 98, 246, 47, 95, 139, 182, 129, 194, 123, 154, 92, 176, 160, 43, 225, 40, 197, 64, 221, 241, 92, 215, 64, 33, 0, 43, 23, 69, 19, 45, 253, 198, 229, 249, 127, 17, 216, 37, 153, 50, 186, 168, 162 };

        //kazim123
        var passwordSalt3 = new byte[] { 174, 222, 90, 210, 27, 13, 26, 160, 176, 57, 91, 39, 224, 32, 135, 218, 59, 222, 74, 61, 12, 41, 215, 48, 59, 181, 35, 162, 42, 142, 223, 232, 224, 172, 216, 100, 255, 252, 82, 87, 138, 99, 90, 181, 169, 189, 219, 44, 46, 161, 190, 185, 145, 56, 27, 69, 79, 138, 117, 62, 193, 77, 101, 124, 35, 4, 133, 97, 27, 239, 210, 160, 152, 223, 205, 92, 141, 5, 252, 162, 186, 38, 248, 210, 252, 119, 53, 66, 33, 157, 253, 74, 164, 131, 117, 233, 172, 99, 167, 200, 54, 59, 162, 8, 126, 247, 95, 97, 143, 181, 226, 132, 117, 168, 71, 54, 38, 229, 15, 196, 150, 93, 138, 167, 89, 254, 27, 124 };
        var passwordHash3 = new byte[] { 186, 31, 96, 48, 95, 78, 158, 166, 85, 180, 187, 134, 246, 224, 115, 214, 111, 80, 51, 192, 138, 129, 200, 8, 1, 31, 183, 138, 110, 232, 23, 68, 117, 97, 147, 141, 33, 52, 101, 79, 162, 46, 153, 107, 207, 73, 111, 234, 192, 119, 232, 192, 21, 66, 179, 216, 87, 72, 216, 181, 95, 59, 109, 162 };

        builder.HasData(
        [
            new(1,"root",passwordSalt1,passwordHash1,false,new DateTime(2000,01,01),true){
                Name="Root",
                Surname="Kullanıcı",
                Phone="5000000001",
                Email="root@mail.com",
                Type=UserTypes.Administator,
                SchoolId=null,
                ConnectionId=null,
                LicenceEndDate=DateTime.MaxValue.Date,
             },
            new(2,"Admin",passwordSalt2,passwordHash2,false,new DateTime(2000,01,01),true){
                Name="Admin",
                Surname="Kullanıcı",
                Phone="5000000002",
                Email="admin@mail.com",
                Type=UserTypes.Administator,
                SchoolId=null,
                ConnectionId=null,
                LicenceEndDate=DateTime.MaxValue.Date,
             },
            new(3,"Okul",passwordSalt2,passwordHash2,false,new DateTime(2000,01,01),true){
                Name="Okul",
                Surname="Kullanıcı",
                Phone="5000000003",
                Email="okul@mail.com",
                Type=UserTypes.School,
                SchoolId=1,
                ConnectionId=null,
                LicenceEndDate=DateTime.MaxValue.Date,
             },
            new(4,"Hoca1",passwordSalt2,passwordHash2,false,new DateTime(2000,01,01),true){
                Name="Öğretmen 1",
                Surname="Kullanıcı",
                Phone="5000000004",
                Email="hoca1@mail.com",
                Type=UserTypes.Teacher,
                SchoolId=1,
                ConnectionId=1,
                LicenceEndDate=DateTime.MaxValue.Date,
             },
            new(5,"Hoca2",passwordSalt2,passwordHash2,false,new DateTime(2000,01,01),true){
                Name="Öğretmen 2",
                Surname="Kullanıcı",
                Phone="5000000005",
                Email="hoca2@mail.com",
                Type=UserTypes.Teacher,
                SchoolId=1,
                ConnectionId=2,
                LicenceEndDate=DateTime.MaxValue.Date,
             },
            new(6,"Öğrenci1",passwordSalt2,passwordHash2,false,new DateTime(2000,01,01),true){
                Name="Öğrenci 1",
                Surname="Kullanıcı",
                Phone="5000000006",
                Email="ogrenci1@mail.com",
                Type=UserTypes.Student,
                SchoolId=1,
                ConnectionId=1,
                LicenceEndDate=DateTime.MaxValue.Date,
             },
            new(7,"Öğrenci2",passwordSalt2,passwordHash2,false,new DateTime(2000,01,01),true){
                Name="Öğrenci 2",
                Surname="Kullanıcı",
                Phone="5000000007",
                Email="ogrenci2@mail.com",
                Type=UserTypes.Student,
                SchoolId=1,
                ConnectionId=2,
                LicenceEndDate=DateTime.MaxValue.Date,
             },
            new(8,"Öğrenci3",passwordSalt2,passwordHash2,false,new DateTime(2000,01,01),true){
                Name="Öğrenci 3",
                Surname="Kullanıcı",
                Phone="5000000008",
                Email="ogrenci3@mail.com",
                Type=UserTypes.Student,
                SchoolId=1,
                ConnectionId=3,
                LicenceEndDate=DateTime.MaxValue.Date,
             },
            new(9,"Öğrenci4",passwordSalt2,passwordHash2,false,new DateTime(2000,01,01),true){
                Name="Öğrenci 4",
                Surname="Kullanıcı",
                Phone="5000000009",
                Email="ogrenci4@mail.com",
                Type=UserTypes.Student,
                SchoolId=1,
                ConnectionId=4,
                LicenceEndDate=DateTime.MaxValue.Date,
             },
            new(10,"Öğrenci5",passwordSalt2,passwordHash2,false,new DateTime(2000,01,01),true){
                Name="Öğrenci 5",
                Surname="Kullanıcı",
                Phone="5000000010",
                Email = "ogrenci5@mail.com",
                Type=UserTypes.Student,
                SchoolId=1,
                ConnectionId=5,
                LicenceEndDate=DateTime.MaxValue.Date,
             },
            new(11,"Öğrenci6",passwordSalt2,passwordHash2,false,new DateTime(2000,01,01),true){
                Name="Öğrenci 6",
                Surname="Kullanıcı",
                Phone="5000000011",
                Email="ogrenci6@mail.com",
                Type=UserTypes.Student,
                SchoolId=1,
                ConnectionId=6,
                LicenceEndDate=DateTime.MaxValue.Date,
             },
            new(12,"ozancank@gmail.com",passwordSalt2,passwordHash2,false,new DateTime(2000,01,01),true){
                Name="Ozan Can",
                Surname="Kösemez",
                Phone="5069151010",
                Email="ozancank@gmail.com",
                Type=UserTypes.Student,
                SchoolId=1,
                ConnectionId=7,
                LicenceEndDate=DateTime.MaxValue.Date,
             },
            new(13,"942alicankesen@gmail.com",passwordSalt2,passwordHash2,false,new DateTime(2000,01,01),true){
                Name="Alican",
                Surname="Kesen",
                Phone="5313914388",
                Email="942alicankesen@gmail.com",
                Type=UserTypes.Student,
                SchoolId=1,
                ConnectionId=8,
                LicenceEndDate=DateTime.MaxValue.Date,
             },
            new(14,"balcan1905@gmail.com",passwordSalt2,passwordHash2,false,new DateTime(2000,01,01),true){
                Name="Eyüp",
                Surname="Balcan",
                Phone="5550593005",
                Email="balcan1905@gmail.com",
                Type=UserTypes.Student,
                SchoolId=1,
                ConnectionId=9,
                LicenceEndDate=DateTime.MaxValue.Date,
             },
            new(15,"kazim",passwordSalt3,passwordHash3,false,new DateTime(2024,09,14),true){
                Name="Kazım",
                Surname="Yıldırım",
                Phone="5413695228",
                Email="kazimyildirimeng@gmail.com",
                Type=UserTypes.Administator,
                SchoolId=null,
                ConnectionId=null,
                LicenceEndDate=DateTime.MaxValue.Date,
             },
            //16 Ahmet Çelik
            //17 Göksel Gündüz
            new(18,"kazim1",passwordSalt3,passwordHash3,false,new DateTime(2024,09,14),true){
                Name="Kazım",
                Surname="Yıldırım",
                Phone="54136952281",
                Email="kazimyildirimeng1@gmail.com",
                Type=UserTypes.Administator,
                SchoolId=null,
                ConnectionId=null,
                LicenceEndDate=DateTime.MaxValue.Date,
             },
            new(19,"kazim2",passwordSalt3,passwordHash3,false,new DateTime(2024,09,14),true){
                Name="Kazım",
                Surname="Yıldırım",
                Phone="54136952282",
                Email="kazimyildirimeng2@gmail.com",
                Type=UserTypes.Administator,
                SchoolId=null,
                ConnectionId=null,
                LicenceEndDate=DateTime.MaxValue.Date,
             },
            new(20,"kazim3",passwordSalt3,passwordHash3,false,new DateTime(2024,09,14),true){
                Name="Kazım",
                Surname="Yıldırım",
                Phone="54136952283",
                Email="kazimyildirimeng3@gmail.com",
                Type=UserTypes.Administator,
                SchoolId=null,
                ConnectionId=null,
                LicenceEndDate=DateTime.MaxValue.Date,
             },
        ]);
    }
}