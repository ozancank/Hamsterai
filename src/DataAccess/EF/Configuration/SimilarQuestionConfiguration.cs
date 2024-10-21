namespace DataAccess.EF.Configuration;

public class SimilarQuestionConfiguration : IEntityTypeConfiguration<Similar>
{
    public void Configure(EntityTypeBuilder<Similar> builder)
    {
        builder.ToTable("SimilarQuestions");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.LessonId).HasColumnName("LessonId").HasColumnOrder(6).IsRequired();
        builder.Property(e => e.QuestionPicture).HasColumnName("QuestionPicture").HasColumnOrder(7).IsRequired();
        builder.Property(e => e.QuestionPictureFileName).HasColumnName("QuestionPictureFileName").HasColumnOrder(8).IsRequired();
        builder.Property(e => e.QuestionPictureExtension).HasColumnName("QuestionPictureExtension").HasMaxLength(10).HasColumnOrder(9).IsRequired();
        builder.Property(e => e.ResponseQuestion).HasColumnName("ResponseQuestion").HasColumnOrder(10).IsRequired();
        builder.Property(e => e.ResponseQuestionFileName).HasColumnName("ResponseQuestionFileName").HasColumnOrder(11).IsRequired();
        builder.Property(e => e.ResponseQuestionExtension).HasColumnName("ResponseQuestionExtension").HasMaxLength(10).HasColumnOrder(12).IsRequired();
        builder.Property(e => e.ResponseAnswer).HasColumnName("ResponseAnswer").HasColumnOrder(13).IsRequired();
        builder.Property(e => e.ResponseAnswerFileName).HasColumnName("ResponseAnswerFileName").HasColumnOrder(14).IsRequired();
        builder.Property(e => e.ResponseAnswerExtension).HasColumnName("ResponseAnswerExtension").HasMaxLength(10).HasColumnOrder(15).IsRequired();
        builder.Property(e => e.Status).HasColumnName("Status").HasColumnOrder(16).IsRequired();
        builder.Property(e => e.IsRead).HasColumnName("IsRead").HasDefaultValue(false).HasColumnOrder(17).IsRequired();
        builder.Property(e => e.SendForQuiz).HasColumnName("SendForQuiz").HasDefaultValue(false).HasColumnOrder(18).IsRequired();
        builder.Property(e => e.TryCount).HasColumnName("TryCount").HasDefaultValue((byte)0).HasColumnOrder(19).IsRequired();
        builder.Property(e => e.GainId).HasColumnName("GainId").HasColumnOrder(20);
        builder.Property(e => e.RightOption).HasColumnName("RightOption").HasMaxLength(1).HasColumnOrder(21);
        builder.Property(e => e.ExcludeQuiz).HasColumnName("ExcludeQuiz").HasDefaultValue(false).HasColumnOrder(22).IsRequired();
        builder.Property(e => e.ExistsVisualContent).HasConversion<bool>().HasColumnName("ExistsVisualContent").HasDefaultValue(false).HasColumnOrder(23).IsRequired();

        builder.HasOne(x => x.User).WithMany(x => x.SimilarQuestions).HasForeignKey(x => x.CreateUser).HasPrincipalKey(x => x.Id);
        builder.HasOne(x => x.Lesson).WithMany(x => x.SimilarQuestions).HasForeignKey(x => x.LessonId).HasPrincipalKey(x => x.Id);
        builder.HasOne(x => x.Gain).WithMany(x => x.SimilarQuestions).HasForeignKey(x => x.GainId).HasPrincipalKey(x => x.Id);
    }
}