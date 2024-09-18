namespace DataAccess.EF.Configuration;

public class TeacherLessonConfiguration : IEntityTypeConfiguration<TeacherLesson>
{
    public void Configure(EntityTypeBuilder<TeacherLesson> builder)
    {
        builder.ToTable("TeacherLessons");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.TeacherId).HasColumnName("TeacherId").HasColumnOrder(6).IsRequired();
        builder.Property(e => e.LessonId).HasColumnName("LessonId").HasColumnOrder(7).IsRequired();
        builder.HasIndex(e => new { e.TeacherId, e.LessonId }).HasDatabaseName("IX_TeacherLessons_1").IsUnique();

        builder.HasOne(d => d.Teacher).WithMany(p => p.TeacherLessons).HasForeignKey(d => d.TeacherId).HasPrincipalKey(x => x.Id);
        builder.HasOne(d => d.Lesson).WithMany(p => p.TeacherLessons).HasForeignKey(d => d.LessonId).HasPrincipalKey(x => x.Id);

        builder.HasData([
            new(Guid.Parse("a1a84a26-a7e4-4671-a979-d65fbbbedec0"),true,2,new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01),1,2),
            new(Guid.Parse("a1a84a26-a7e4-4671-a979-d65fbbbedec1"),true,2,new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01),2,1),
        ]);
    }
}