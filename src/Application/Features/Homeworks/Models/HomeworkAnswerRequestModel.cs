namespace Application.Features.Homeworks.Models;

public class HomeworkAnswerRequestModel : IRequestModel
{
    public string? HomeworkStudentId { get; set; }
    public string? AnswerPictureBase64 { get; set; }
    public string? AnswerPictureFileName { get; set; }
}