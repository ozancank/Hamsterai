namespace DataAccess.EF.Configuration;

public class HomeworkConfiguration : IEntityTypeConfiguration<Homework>
{
    public void Configure(EntityTypeBuilder<Homework> builder)
    {
        builder.ToTable("Homeworks");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.SchoolId).HasColumnName("SchoolId").HasColumnOrder(6);
        builder.Property(e => e.TeacherId).HasColumnName("TeacherId").HasColumnOrder(7);
        builder.Property(e => e.LessonId).HasColumnName("LessonId").HasColumnOrder(8).IsRequired();
        builder.Property(e => e.FilePath).HasColumnName("FilePath").HasColumnOrder(9).IsRequired();
        builder.Property(e => e.ClassRoomId).HasColumnName("ClassRoomId").HasColumnOrder(10);
        builder.Property(e => e.Title).HasColumnName("Title").HasMaxLength(200).HasColumnOrder(11).IsRequired();
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnOrder(12);

        builder.HasOne(x => x.User).WithMany(x => x.Homeworks).HasForeignKey(x => x.CreateUser).HasPrincipalKey(x => x.Id);
        builder.HasOne(x => x.School).WithMany(x => x.Homeworks).HasForeignKey(x => x.SchoolId).HasPrincipalKey(x => x.Id);
        builder.HasOne(x => x.Teacher).WithMany(x => x.Homeworks).HasForeignKey(x => x.TeacherId).HasPrincipalKey(x => x.Id);
        builder.HasOne(x => x.Lesson).WithMany(x => x.Homeworks).HasForeignKey(x => x.LessonId).HasPrincipalKey(x => x.Id);
        builder.HasOne(x => x.ClassRoom).WithMany(x => x.Homeworks).HasForeignKey(x => x.ClassRoomId).HasPrincipalKey(x => x.Id);
    }
}