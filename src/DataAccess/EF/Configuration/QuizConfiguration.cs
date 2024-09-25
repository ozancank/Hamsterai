namespace DataAccess.EF.Configuration;

public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.ToTable("Quizzes");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasMaxLength(100).HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.UserId).HasColumnName("UserId").HasColumnOrder(6).IsRequired();
        builder.Property(e => e.LessonId).HasColumnName("LessonId").HasColumnOrder(7).IsRequired();
        builder.Property(e => e.TimeSecond).HasColumnName("TimeSecond").HasColumnOrder(8).IsRequired();
        builder.Property(e => e.Status).HasColumnName("Status").HasColumnOrder(9).IsRequired();
        builder.Property(e => e.CorrectCount).HasColumnName("CorrectCount").HasColumnOrder(10).IsRequired();
        builder.Property(e => e.WrongCount).HasColumnName("WrongCount").HasColumnOrder(11).IsRequired();
        builder.Property(e => e.EmptyCount).HasColumnName("EmptyCount").HasColumnOrder(12).IsRequired();
        builder.Property(e => e.SuccessRate).HasColumnName("SuccessRate").HasColumnOrder(13).IsRequired();

        builder.HasOne(x => x.User).WithMany(x => x.Quizzes).HasForeignKey(x => x.UserId).HasPrincipalKey(x => x.Id);
        builder.HasOne(x => x.Lesson).WithMany(x => x.Quizzes).HasForeignKey(x => x.LessonId).HasPrincipalKey(x => x.Id);
    }
}