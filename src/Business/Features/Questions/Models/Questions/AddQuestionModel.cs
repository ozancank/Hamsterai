namespace Business.Features.Questions.Models.Questions;

public class AddQuestionModel : IRequestModel
{
    public byte LessonId { get; set; }
    public string? QuestionPictureBase64 { get; set; }
    public string? QuestionPictureFileName { get; set; }
}