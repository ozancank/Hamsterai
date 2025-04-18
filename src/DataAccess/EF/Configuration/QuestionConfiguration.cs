﻿namespace DataAccess.EF.Configuration;

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
        builder.Property(e => e.QuestionText).HasColumnName("QuestionText").HasColumnOrder(7).IsRequired();
        builder.Property(e => e.QuestionPictureFileName).HasColumnName("QuestionPictureFileName").HasColumnOrder(8).IsRequired();
        builder.Property(e => e.QuestionPictureExtension).HasColumnName("QuestionPictureExtension").HasMaxLength(10).HasColumnOrder(9).IsRequired();
        builder.Property(e => e.AnswerText).HasColumnName("AnswerText").HasColumnOrder(10).IsRequired();
        builder.Property(e => e.Status).HasColumnName("Status").HasColumnOrder(11).IsRequired();
        builder.Property(e => e.AnswerPictureFileName).HasColumnName("AnswerPictureFileName").HasDefaultValue(string.Empty).HasColumnOrder(12).IsRequired();
        builder.Property(e => e.AnswerPictureExtension).HasColumnName("AnswerPictureExtension").HasMaxLength(10).HasDefaultValue("").HasColumnOrder(13).IsRequired();
        builder.Property(e => e.IsRead).HasColumnName("IsRead").HasDefaultValue(false).HasColumnOrder(14).IsRequired();
        builder.Property(e => e.SendForQuiz).HasColumnName("SendForQuiz").HasDefaultValue(false).HasColumnOrder(15).IsRequired();
        builder.Property(e => e.TryCount).HasColumnName("TryCount").HasDefaultValue((byte)0).HasColumnOrder(16).IsRequired();
        builder.Property(e => e.GainId).HasColumnName("GainId").HasColumnOrder(17);
        builder.Property(e => e.RightOption).HasColumnName("RightOption").HasMaxLength(1).HasColumnOrder(18);
        builder.Property(e => e.ExcludeQuiz).HasColumnName("ExcludeQuiz").HasDefaultValue(false).HasColumnOrder(19).IsRequired();
        builder.Property(e => e.ExistsVisualContent).HasColumnName("ExistsVisualContent").HasDefaultValue(false).HasColumnOrder(20).IsRequired();
        builder.Property(e => e.OcrMethod).HasColumnName("OcrMethod").HasMaxLength(50).HasDefaultValue(string.Empty).HasColumnOrder(21).IsRequired();
        builder.Property(e => e.ErrorDescription).HasColumnName("ErrorDescription").HasDefaultValue(string.Empty).HasColumnOrder(22);
        builder.Property(e => e.AIIP).HasColumnName("AIIP").HasMaxLength(50).HasDefaultValue(string.Empty).HasColumnOrder(23);
        builder.Property(e => e.ReadDate).HasColumnName("ReadDate").HasDefaultValue(AppStatics.MilleniumDate).HasColumnOrder(24).IsRequired();
        builder.Property(e => e.SendQuizDate).HasColumnName("SendQuizDate").HasDefaultValue(AppStatics.MilleniumDate).HasColumnOrder(25).IsRequired();
        builder.Property(e => e.SimilarId).HasColumnName("SimilarId").HasColumnOrder(26);
        builder.Property(e => e.ManuelSendAgain).HasColumnName("ManuelSendAgain").HasDefaultValue(false).HasColumnOrder(27).IsRequired();
        builder.Property(e => e.Type).HasColumnName("Type").HasDefaultValue(QuestionType.Question).HasColumnOrder(28).IsRequired();

        builder.HasOne(x => x.User).WithMany(x => x.Questions).HasForeignKey(x => x.CreateUser).HasPrincipalKey(x => x.Id);
        builder.HasOne(x => x.Lesson).WithMany(x => x.Questions).HasForeignKey(x => x.LessonId).HasPrincipalKey(x => x.Id);
        builder.HasOne(x => x.Gain).WithMany(x => x.Questions).HasForeignKey(x => x.GainId).HasPrincipalKey(x => x.Id);
    }
}