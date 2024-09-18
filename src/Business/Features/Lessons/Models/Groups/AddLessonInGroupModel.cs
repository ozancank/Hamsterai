namespace Business.Features.Lessons.Models.Groups;

public sealed class AddLessonInGroupModel : IResponseModel
{
    public byte GroupId { get; set; }
    public List<byte> LessonIds { get; set; }
}