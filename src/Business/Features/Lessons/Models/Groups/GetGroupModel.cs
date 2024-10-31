using Business.Features.Lessons.Models.Lessons;

namespace Business.Features.Lessons.Models.Groups;

public sealed class GetGroupModel : IResponseModel
{
    public byte Id { get; set; }
    public bool IsActive { get; set; }
    public long CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public long UpdateUser { get; set; }
    public DateTime UpdateDate { get; set; }
    public string Name { get; set; }
    public byte SortNo { get; set; }
    public bool IsWebVisible { get; set; }

    public List<byte> LessonIds { get; set; } = [];
    public List<GetLessonLiteModel> Lessons { get; set; }
}