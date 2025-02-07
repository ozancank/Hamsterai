using Domain.Entities.Core;

namespace Domain.Entities;

public class QuizQuestion : BaseEntity<string>
{
    public byte SortNo { get; set; }
    public string? QuizId { get; set; }
    public string? Question { get; set; }
    public string? QuestionPictureFileName { get; set; }
    public string? QuestionPictureExtension { get; set; }
    public string? Answer { get; set; }
    public string? AnswerPictureFileName { get; set; }
    public string? AnswerPictureExtension { get; set; }
    public char RightOption { get; set; }
    public char? AnswerOption { get; set; }
    public byte OptionCount { get; set; }
    public int? GainId { get; set; }

    public virtual Quiz? Quiz { get; set; }
    public virtual Gain? Gain { get; set; }

    public QuizQuestion() : base()
    { }

    public QuizQuestion(string id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    { }
}