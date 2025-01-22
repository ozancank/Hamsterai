namespace DataAccess.EF.Configuration;

public class PostitConfiguration : IEntityTypeConfiguration<Postit>
{
    public void Configure(EntityTypeBuilder<Postit> builder)
    {
        builder.ToTable("Postits");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.LessonId).HasColumnName("LessonId").HasColumnOrder(6).IsRequired();
        builder.Property(e => e.Title).HasColumnName("Title").HasMaxLength(250).HasDefaultValue(string.Empty).HasColumnOrder(7);
        builder.Property(e => e.Description).HasColumnName("Description").HasDefaultValue(string.Empty).HasColumnOrder(8);
        builder.Property(e => e.Color).HasColumnName("Color").HasMaxLength(8).HasDefaultValue(Strings.PostitDefaultColor).HasColumnOrder(9);
        builder.Property(e => e.SortNo).HasColumnName("SortNo").HasColumnOrder(10).IsRequired();
        builder.Property(e => e.PictureFileName).HasColumnName("PictureFileName").HasDefaultValue(string.Empty).HasColumnOrder(11);

        builder.HasOne(d => d.Lesson).WithMany(p => p.Postits).HasForeignKey(d => d.LessonId).HasPrincipalKey(x => x.Id);
        builder.HasOne(d => d.User).WithMany(p => p.Postits).HasForeignKey(d => d.CreateUser).HasPrincipalKey(x => x.Id);
    }
}