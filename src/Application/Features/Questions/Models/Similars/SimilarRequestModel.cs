namespace Application.Features.Questions.Models.Similars;

public class QuestionRequestModel : IRequestModel
{
    public short LessonId { get; set; } = 0;
    public DateTime? StartDate { get; set; } = null;
    public DateTime? EndDate { get; set; } = null;
}