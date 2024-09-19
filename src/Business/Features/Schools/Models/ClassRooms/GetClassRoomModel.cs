using Business.Features.Schools.Models.Schools;
using Business.Features.Students.Models;
using Business.Features.Teachers.Models;

namespace Business.Features.Schools.Models.ClassRooms;

public sealed class GetClassRoomModel : IResponseModel
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public long CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public long UpdateUser { get; set; }
    public DateTime UpdateDate { get; set; }
    public short No { get; set; }
    public string Branch { get; set; }
    public int SchoolId { get; set; }

    public GetSchoolModel School { get; set; }
    public List<GetTeacherModel> Teachers { get; set; } = [];
    public List<GetStudentModel> Students { get; set; } = [];
}