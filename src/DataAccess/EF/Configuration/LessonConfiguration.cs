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
        builder.HasAlternateKey(e => e.Name).HasName("UK_Lessons_Name");
        builder.Property(e => e.SortNo).HasColumnName("SortNo").HasColumnOrder(7).IsRequired();

        builder.HasData([
                new(1, true, 2, new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01), "Türkçe",1),
                new(2, true, 2, new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01), "Matematik",2),
            ]);
    }
}