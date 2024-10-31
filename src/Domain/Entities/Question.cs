using Domain.Constants;
using Domain.Entities.Core;

namespace Domain.Entities;

public class Question : BaseEntity<Guid>
{
    public byte LessonId { get; set; }
    public string? QuestionPictureBase64 { get; set; }
    public string? QuestionPictureFileName { get; set; }
    public string? QuestionPictureExtension { get; set; }
    public string? AnswerText { get; set; }
    public string? AnswerPictureFileName { get; set; }
    public string? AnswerPictureExtension { get; set; }
    public QuestionStatus Status { get; set; }
    public bool IsRead { get; set; }
    public bool SendForQuiz { get; set; }
    public byte TryCount { get; set; }
    public int? GainId { get; set; }
    public char? RightOption { get; set; }
    public bool ExcludeQuiz { get; set; }
    public bool ExistsVisualContent { get; set; }
    public string? OcrMethod { get; set; }
    public string? ErrorDescription { get; set; }
    public string? AIIP { get; set; }

    public virtual User? User { get; set; }
    public virtual Lesson? Lesson { get; set; }
    public virtual Gain? Gain { get; set; }

    public Question() : base()
    { }

    public Question(Guid id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    { }
}