namespace DataAccess.EF.Configuration;

public class QuizQuestionConfiguration : IEntityTypeConfiguration<QuizQuestion>
{
    public void Configure(EntityTypeBuilder<QuizQuestion> builder)
    {
        builder.ToTable("QuizQuestions");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasMaxLength(100).HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.SortNo).HasColumnName("SortNo").HasColumnOrder(6).IsRequired();
        builder.Property(e => e.QuizId).HasColumnName("QuizId").HasColumnOrder(7).IsRequired();
        builder.Property(e => e.Question).HasColumnName("Question").HasColumnOrder(8).IsRequired();
        builder.Property(e => e.QuestionPictureFileName).HasColumnName("QuestionPictureFileName").HasColumnOrder(9).IsRequired();
        builder.Property(e => e.QuestionPictureExtension).HasColumnName("QuestionPictureExtension").HasMaxLength(10).HasColumnOrder(10).IsRequired();
        builder.Property(e => e.Answer).HasColumnName("Answer").HasColumnOrder(11).IsRequired();
        builder.Property(e => e.AnswerPictureFileName).HasColumnName("AnswerPictureFileName").HasColumnOrder(12).IsRequired();
        builder.Property(e => e.AnswerPictureExtension).HasColumnName("AnswerPictureExtension").HasMaxLength(10).HasColumnOrder(13).IsRequired();
        builder.Property(e => e.RightOption).HasColumnName("RightOption").HasMaxLength(1).HasColumnOrder(14).IsRequired();
        builder.Property(e => e.AnswerOption).HasColumnName("AnswerOption").HasMaxLength(1).HasColumnOrder(15);
        builder.Property(e => e.OptionCount).HasColumnName("OptionCount").HasColumnOrder(16).IsRequired();
        builder.Property(e => e.GainId).HasColumnName("GainId").HasColumnOrder(17).IsRequired();

        builder.HasOne(x => x.Quiz).WithMany(x => x.QuizQuestions).HasForeignKey(x => x.QuizId).HasPrincipalKey(x => x.Id);
        builder.HasOne(x => x.Gain).WithMany(x => x.QuizQuestions).HasForeignKey(x => x.GainId).HasPrincipalKey(x => x.Id);
    }
}