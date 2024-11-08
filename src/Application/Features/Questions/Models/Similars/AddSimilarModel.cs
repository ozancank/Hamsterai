namespace Application.Features.Questions.Models.Similars;

public class AddSimilarModel : IRequestModel
{
    public short LessonId { get; set; }
    public string? QuestionPictureBase64 { get; set; }
    public string? QuestionPictureFileName { get; set; }
}