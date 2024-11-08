using Application.Features.Schools.Models.Schools;
using Application.Features.Students.Models;
using Application.Features.Teachers.Models;

namespace Application.Features.Schools.Models.ClassRooms;

public sealed class GetClassRoomModel : IResponseModel
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public long CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public long UpdateUser { get; set; }
    public DateTime UpdateDate { get; set; }
    public short No { get; set; }
    public string? Branch { get; set; }
    public int SchoolId { get; set; }
    public short PackageId { get; set; }
    public string? PackageName { get; set; }

    public GetSchoolModel? School { get; set; }
    public List<GetTeacherModel> Teachers { get; set; } = [];
    public List<GetStudentModel> Students { get; set; } = [];
}