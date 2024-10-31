namespace Business.Features.Teachers.Models;

public class AssignTeacherClassRoomModel : IRequestModel
{
    public int TeacherId { get; set; }
    public List<int> ClassRoomIds { get; set; } = [];
}