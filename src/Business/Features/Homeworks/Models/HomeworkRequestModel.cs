namespace Business.Features.Homeworks.Models;

public class HomeworkRequestModel : IRequestModel
{
    public byte LessonId { get; set; } = 0;
    public DateTime? StartDate { get; set; } = null;
    public DateTime? EndDate { get; set; } = null;
}