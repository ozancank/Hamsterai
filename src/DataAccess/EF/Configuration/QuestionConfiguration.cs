namespace DataAccess.EF.Configuration;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.LessonId).HasColumnName("LessonId").HasColumnOrder(6).IsRequired();
        builder.Property(e => e.QuestionPictureBase64).HasColumnName("QuestionPictureBase64").HasColumnOrder(7).IsRequired();
        builder.Property(e => e.QuestionPictureFileName).HasColumnName("QuestionPictureFileName").HasColumnOrder(8).IsRequired();
        builder.Property(e => e.QuestionPictureExtension).HasColumnName("QuestionPictureExtension").HasMaxLength(10).HasColumnOrder(9).IsRequired();
        builder.Property(e => e.AnswerText).HasColumnName("AnswerText").HasColumnOrder(10).IsRequired();
        builder.Property(e => e.Status).HasColumnName("Status").HasColumnOrder(11).IsRequired();
        builder.Property(e => e.AnswerPictureFileName).HasColumnName("AnswerPictureFileName").HasDefaultValue("").HasColumnOrder(12).IsRequired();
        builder.Property(e => e.AnswerPictureExtension).HasColumnName("AnswerPictureExtension").HasMaxLength(10).HasDefaultValue("").HasColumnOrder(13).IsRequired();

        builder.HasOne(x => x.User).WithMany(x => x.Questions).HasForeignKey(x => x.CreateUser).HasPrincipalKey(x => x.Id);
        builder.HasOne(x => x.Lesson).WithMany(x => x.Questions).HasForeignKey(x => x.LessonId).HasPrincipalKey(x => x.Id);
    }
}