namespace Business.Features.Schools.Models.ClassRooms;

public class AddClassRoomModel : IRequestModel
{
    public short No { get; set; }
    public string Branch { get; set; }
    public byte GroupId { get; set; }
}