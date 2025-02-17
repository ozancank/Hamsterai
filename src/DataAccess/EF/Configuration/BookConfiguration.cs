namespace DataAccess.EF.Configuration;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasColumnType("citext").HasMaxLength(100).HasColumnOrder(6).IsRequired();
        builder.Property(e => e.LessonId).HasColumnName("LessonId").HasColumnOrder(7).IsRequired();
        builder.Property(e => e.PageCount).HasColumnName("PageCount").HasColumnOrder(8).IsRequired();
        builder.Property(e => e.PublisherId).HasColumnName("PublisherId").HasColumnOrder(9).IsRequired();
        builder.Property(e => e.Year).HasColumnName("Year").HasColumnOrder(10);
        builder.Property(e => e.SchoolId).HasColumnName("SchoolId").HasColumnOrder(11).IsRequired();
        builder.Property(e => e.ThumbBase64).HasColumnName("ThumbBase64").HasColumnName("citext").HasColumnOrder(12);
        builder.Property(e => e.TryPrepareCount).HasColumnName("TryPrepareCount").HasDefaultValue((byte)0).HasColumnOrder(13).IsRequired();
        builder.Property(e => e.Status).HasColumnName("Status").HasDefaultValue(BookStatus.Unknown).HasColumnOrder(14).IsRequired();

        builder.HasOne(d => d.School).WithMany(p => p.Books).HasForeignKey(d => d.SchoolId).HasPrincipalKey(x => x.Id);
        builder.HasOne(d => d.Publisher).WithMany(p => p.Books).HasForeignKey(d => d.PublisherId).HasPrincipalKey(x => x.Id);
    }
}