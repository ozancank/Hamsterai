namespace Business.Features.Lessons.Models.Groups;

public sealed class GetGroupLiteModel : IResponseModel
{
    public byte Id { get; set; }
    public string Name { get; set; }
    public List<byte> LessonIds { get; set; } = [];
}