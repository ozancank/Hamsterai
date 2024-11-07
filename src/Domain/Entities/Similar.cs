using Domain.Constants;
using Domain.Entities.Core;

namespace Domain.Entities;

public class Similar : BaseEntity<Guid>
{
    public short LessonId { get; set; }
    public string? QuestionPicture { get; set; }
    public string? QuestionPictureFileName { get; set; }
    public string? QuestionPictureExtension { get; set; }
    public string? ResponseQuestion { get; set; }
    public string? ResponseQuestionFileName { get; set; }
    public string? ResponseQuestionExtension { get; set; }
    public string? ResponseAnswer { get; set; }
    public string? ResponseAnswerFileName { get; set; }
    public string? ResponseAnswerExtension { get; set; }
    public QuestionStatus Status { get; set; }
    public bool IsRead { get; set; }
    public bool SendForQuiz { get; set; }
    public int TryCount { get; set; }
    public int? GainId { get; set; }
    public char? RightOption { get; set; }
    public bool ExcludeQuiz { get; set; }
    public bool ExistsVisualContent { get; set; }
    public string? ErrorDescription { get; set; }
    public string? AIIP { get; set; }

    public virtual User? User { get; set; }
    public virtual Lesson? Lesson { get; set; }
    public virtual Gain? Gain { get; set; }
}