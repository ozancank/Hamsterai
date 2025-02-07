namespace Application.Features.Questions.Models.Questions;

public class QuestionForAdminRequestModel : IRequestModel
{
    public short LessonId { get; set; } = 0;
    public DateTime? StartDate { get; set; } = null;
    public DateTime? EndDate { get; set; } = null;
    public bool OnlyError { get; set; } = false;
    public byte MinTryCount { get; set; } = 0;
    public bool OnlyCustomer { get; set; } = true;
}