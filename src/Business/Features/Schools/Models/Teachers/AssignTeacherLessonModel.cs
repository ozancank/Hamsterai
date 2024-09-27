namespace Business.Features.Schools.Models.Teachers;

public class AssignTeacherLessonModel : IRequestModel
{
    public int TeacherId { get; set; }
    public List<byte> LessonIds { get; set; }
}