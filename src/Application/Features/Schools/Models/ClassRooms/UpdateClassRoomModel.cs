namespace Application.Features.Schools.Models.ClassRooms;

public class UpdateClassRoomModel : IRequestModel
{
    public int Id { get; set; }
    public short No { get; set; }
    public string? Branch { get; set; }
    public int SchoolId { get; set; }
    public short PackageId { get; set; }
}