namespace Business.Features.Questions.Models.Similars;

public class GetSimilarModel : IResponseModel
{
    public Guid Id { get; set; }
    public long CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public byte LessonId { get; set; }
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
    public int TryCount { get; set; }
    public int? GainId { get; set; }
    public char? RightOption { get; set; }
    public bool ExcludeQuiz { get; set; }
    public bool ExistsVisualContent { get; set; }
    public string? ErrorDescription { get; set; }
    public string? AIIP { get; set; }

    public string? LessonName { get; set; }
    public string? GainName { get; set; }
}