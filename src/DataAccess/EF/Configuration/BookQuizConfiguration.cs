namespace DataAccess.EF.Configuration;

public class BookQuizConfiguration : IEntityTypeConfiguration<BookQuiz>
{
    public void Configure(EntityTypeBuilder<BookQuiz> builder)
    {
        builder.ToTable("BookQuizzes");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.LessonId).HasColumnName("LessonId").HasColumnOrder(7).IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasColumnType("citext").HasMaxLength(100).HasColumnOrder(6).IsRequired();
        builder.Property(e => e.QuestionCount).HasColumnName("QuestionCount").HasColumnOrder(8).IsRequired();
        builder.Property(e => e.OptionCount).HasColumnName("OptionCount").HasColumnOrder(9).IsRequired();
        builder.Property(e => e.Answers).HasColumnName("Answers").HasColumnType("citext").HasColumnOrder(10).IsRequired();

        builder.HasOne(d => d.Book).WithMany(p => p.BookQuizzes).HasForeignKey(d => d.BookId).HasPrincipalKey(x => x.Id);
        builder.HasOne(d => d.Lesson).WithMany(p => p.BookQuizzes).HasForeignKey(d => d.LessonId).HasPrincipalKey(x => x.Id);
    }
}