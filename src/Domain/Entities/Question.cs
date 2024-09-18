using Domain.Constants;
using Domain.Entities.Core;

namespace Domain.Entities;

public class Question : BaseEntity<Guid>
{
    public byte LessonId { get; set; }
    public string QuestionPictureBase64 { get; set; }
    public string QuestionPictureFileName { get; set; }
    public string QuestionPictureExtension { get; set; }
    public string AnswerText { get; set; }
    public string AnswerPictureFileName { get; set; }
    public string AnswerPictureExtension { get; set; }
    public QuestionStatus Status { get; set; }

    public virtual User User { get; set; }
    public virtual Lesson Lesson { get; set; }
}