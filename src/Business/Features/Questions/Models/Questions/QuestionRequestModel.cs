namespace Business.Features.Questions.Models.Questions;

public class SimilarQuestionRequestModel : IRequestModel
{
    public byte LessonId { get; set; } = 0;
    public DateTime? StartDate { get; set; } = null;
    public DateTime? EndDate { get; set; } = null;
}