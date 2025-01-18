namespace Application.Features.Questions.Models.Questions;

public class AddQuestionModel : IRequestModel
{
    public short LessonId { get; set; }
    public string? QuestionPictureBase64 { get; set; }
    public string? QuestionSmallPictureBase64 { get; set; }
    public string? QuestionPictureFileName { get; set; }
    public bool IsVisual { get; set; }
    public QuestionType Type { get; set; } = QuestionType.Question;
}