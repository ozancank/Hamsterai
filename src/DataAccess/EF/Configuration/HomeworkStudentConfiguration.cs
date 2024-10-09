namespace DataAccess.EF.Configuration;

public class HomeworkStudentConfiguration : IEntityTypeConfiguration<HomeworkStudent>
{
    public void Configure(EntityTypeBuilder<HomeworkStudent> builder)
    {
        builder.ToTable("HomeworkStudents");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.StudentId).HasColumnName("StudentId").HasColumnOrder(6).IsRequired();
        builder.Property(e => e.HomeworkId).HasColumnName("HomeworkId").HasColumnOrder(7).IsRequired();
        builder.Property(e => e.AnswerPath).HasColumnName("AnswerPath").HasColumnOrder(8);
        builder.Property(e => e.Status).HasColumnName("Status").HasColumnOrder(9).IsRequired();

        builder.HasOne(x => x.Student).WithMany(x => x.HomeworkStudents).HasForeignKey(x => x.StudentId).HasPrincipalKey(x => x.Id);
        builder.HasOne(x => x.Homework).WithMany(x => x.HomeworkStudents).HasForeignKey(x => x.HomeworkId).HasPrincipalKey(x => x.Id);
    }
}