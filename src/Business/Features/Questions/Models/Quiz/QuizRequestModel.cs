namespace Business.Features.Questions.Models.Similars;

public class QuizRequestModel : IRequestModel
{
    public byte LessonId { get; set; } = 0;
    public DateTime? StartDate { get; set; } = null;
    public DateTime? EndDate { get; set; } = null;
}