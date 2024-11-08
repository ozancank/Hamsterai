namespace Application.Features.Schools.Models.ClassRooms;

public class AddClassRoomModel : IRequestModel
{
    public short No { get; set; }
    public string? Branch { get; set; }
    public short PackageId { get; set; }
}