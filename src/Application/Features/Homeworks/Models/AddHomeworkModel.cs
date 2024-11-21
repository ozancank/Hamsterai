namespace Application.Features.Homeworks.Models;

public class AddHomeworkModel : IRequestModel
{
    public short LessonId { get; set; }
    public IFormFile? File { get; set; }
    public int? ClassRoomId { get; set; }
    public List<int> StudentIds { get; set; } = [];
    public List<long> UserIds { get; set; } = [];
    public List<short> PackageIds { get; set; } = [];
}