namespace Application.Features.Questions.Models.Questions;

public class GetQuestionForAdminModel : IResponseModel
{
    public Guid Id { get; set; }
    public DateTime CreateDate { get; set; }
    public string? OcrMethod { get; set; }
    public string? UserName { get; set; }
    public string? LessonName { get; set; }
    public string? Status { get; set; }
    public byte TryCount { get; set; }
    public string? AIIP { get; set; }
    public Guid? SimilarId { get; set; }
    public int GainId { get; set; }
    public string? GainName { get; set; }
    public bool ExistsVisualContent { get; set; }
    public bool SendForQuiz { get; set; }
    public bool IsRead { get; set; }
    public bool ManuelSendAgain { get; set; }
    public string? QuestionPicture { get; set; }
    public string? QuestionText { get; set; }
    public string? AnswerText { get; set; }
    public string? ErrorDescription { get; set; }
}