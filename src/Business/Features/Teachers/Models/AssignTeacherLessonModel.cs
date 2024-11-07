namespace Business.Features.Teachers.Models;

public class AssignTeacherLessonModel : IRequestModel
{
    public int TeacherId { get; set; }
    public List<short> LessonIds { get; set; } = [];
}