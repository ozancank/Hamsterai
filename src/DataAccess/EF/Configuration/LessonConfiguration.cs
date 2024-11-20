namespace DataAccess.EF.Configuration;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("Lessons");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasMaxLength(50).HasColumnOrder(6).IsRequired();
        builder.HasIndex(e => e.Name).HasDatabaseName("IX_Lesson_Name").IsUnique();
        builder.Property(e => e.SortNo).HasColumnName("SortNo").HasColumnOrder(7).IsRequired();

        builder.HasData([
                new(1, true, 2, new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01), "Türkçe",1),
                new(2, true, 2, new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01), "Matematik",2),
                new(3, true, 2, new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01), "İngilizce",3),
                new(4, true, 2, new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01), "Fen Bilimleri",4),
                new(5, true, 2, new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01), "Fizik",5),
                new(6, true, 2, new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01), "Kimya",6),
                new(7, true, 2, new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01), "Biyoloji",7),
                new(8, true, 2, new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01), "Edebiyat",8),
                new(9, true, 2, new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01), "Tarih",9),
                new(10, true, 2, new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01), "Coğrafya",10),
                new(11, true, 2, new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01), "Felsefe",11),
                new(12, true, 2, new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01), "Din Kültürü",12),
            ]);
    }
}