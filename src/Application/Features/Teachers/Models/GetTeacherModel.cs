using Application.Features.Lessons.Models.Lessons;
using Application.Features.Schools.Models.ClassRooms;

namespace Application.Features.Teachers.Models;

public sealed class GetTeacherModel : IResponseModel
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public long CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public long UpdateUser { get; set; }
    public DateTime UpdateDate { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? TcNo { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Branch { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public string? FullName { get; set; }
    public long UserId { get; set; }

    public List<GetClassRoomModel> ClassRooms { get; set; } = [];
    public List<GetLessonModel> Lessons { get; set; } = [];
}