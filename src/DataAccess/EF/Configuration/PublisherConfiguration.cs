namespace DataAccess.EF.Configuration;

public class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
{
    public void Configure(EntityTypeBuilder<Publisher> builder)
    {
        builder.ToTable("Publishers");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasColumnType("citext").HasMaxLength(100).HasColumnOrder(6).IsRequired();
        builder.HasIndex(e => e.Name).HasDatabaseName("IX_Publishers_Name").IsUnique();

        builder.HasData([
                new(1,true,2,AppStatics.MilleniumDate,2,AppStatics.MilleniumDate,"Hız Yayınları"), 
                new(2,true,2,AppStatics.MilleniumDate,2,AppStatics.MilleniumDate,"3D Yayınları"),
                new(3,true,2,AppStatics.MilleniumDate,2,AppStatics.MilleniumDate,"Vip Yayınları"),
                new(4,true,2,AppStatics.MilleniumDate,2,AppStatics.MilleniumDate,"Okyanus Yayınları"),
                new(5,true,2,AppStatics.MilleniumDate,2,AppStatics.MilleniumDate,"Paraf Yayınları"),
                new(6,true,2,AppStatics.MilleniumDate,2,AppStatics.MilleniumDate,"Yanıt Yayınları"),
                new(7,true,2,AppStatics.MilleniumDate,2,AppStatics.MilleniumDate,"Acil Yayınları"),
                new(8,true,2,AppStatics.MilleniumDate,2,AppStatics.MilleniumDate,"Miray Yayınları"),
                new(9,true,2,AppStatics.MilleniumDate,2,AppStatics.MilleniumDate,"Palme Yayınları"),
                new(10,true,2,AppStatics.MilleniumDate,2,AppStatics.MilleniumDate,"Muba Yayınları"),
                new(11,true,2,AppStatics.MilleniumDate,2,AppStatics.MilleniumDate,"Arı Yayınları"),
                new(12,true,2,AppStatics.MilleniumDate,2,AppStatics.MilleniumDate,"Nitelik Yayınları"),
                new(13,true,2,AppStatics.MilleniumDate,2,AppStatics.MilleniumDate,"Fenomen Yayınları"),
                new(14,true,2,AppStatics.MilleniumDate,2,AppStatics.MilleniumDate,"Ankara Yayınları"),
                new(15,true,2,AppStatics.MilleniumDate,2,AppStatics.MilleniumDate,"Adrenalin Yayınları"),
                new(16,true,2,AppStatics.MilleniumDate,2,AppStatics.MilleniumDate,"Paylaşım Yayınları"),
                new(17,true,2,AppStatics.MilleniumDate,2,AppStatics.MilleniumDate,"Ogm Yayınları"),
                new(18,true,2,AppStatics.MilleniumDate,2,AppStatics.MilleniumDate,"Bilgi Sarmal Yayınları"),
                new(19,true,2,AppStatics.MilleniumDate,2,AppStatics.MilleniumDate,"Çap Yayınları"),
                new(20,true,2,AppStatics.MilleniumDate,2,AppStatics.MilleniumDate,"Aktif Yayınları"),
                new(21,true,2,AppStatics.MilleniumDate,2,AppStatics.MilleniumDate,"Aydın Yayınları"),
                new(22,true,2,AppStatics.MilleniumDate,2,AppStatics.MilleniumDate,"Biotik Yayıları"),
            ]);
    }
}