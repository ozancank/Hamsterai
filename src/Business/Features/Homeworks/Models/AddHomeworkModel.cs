namespace Business.Features.Homeworks.Models;

public class AddHomeworkModel : IModel
{
    public byte LessonId { get; set; }
    public IFormFile? File { get; set; }
    public int? ClassRoomId { get; set; }
    public List<int> StudentIds { get; set; } = [];
}