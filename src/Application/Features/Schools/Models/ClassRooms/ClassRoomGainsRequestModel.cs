namespace Application.Features.Schools.Models.ClassRooms;

public class ClassRoomGainsRequestModel : IRequestModel
{
    public int ClassRoomId { get; set; } = 0;
    public DateTime? StartDate { get; set; } = null;
    public DateTime? EndDate { get; set; } = null;
}