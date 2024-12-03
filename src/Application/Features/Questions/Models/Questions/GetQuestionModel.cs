namespace Application.Features.Questions.Models.Questions;

public class GetQuestionModel : IResponseModel
{
    public Guid Id { get; set; }
    public long CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public short LessonId { get; set; }
    public LessonTypes LessonType { get; set; }
    public string? QuestionPictureFileName { get; set; }
    public string? QuestionPictureExtension { get; set; }
    public string? AnswerText { get; set; }
    public QuestionStatus Status { get; set; }
    public bool IsRead { get; set; }
    public byte TryCount { get; set; }
    public int GainId { get; set; }
    public char? RightOption { get; set; }
    public bool ExistsVisualContent { get; set; }
    public string? OcrMethod { get; set; }
    public string? ErrorDescription { get; set; }
    public string? AIIP { get; set; }
    public bool ManuelSendAgain { get; set; }

    public string? LessonName { get; set; }
    public string? GainName { get; set; }
}