namespace Business.Features.Questions.Models.Quizzes;

public class QuizRequestModel : IRequestModel
{
    public byte LessonId { get; set; } = 0;
    public DateTime? StartDate { get; set; } = null;
    public DateTime? EndDate { get; set; } = null;
}