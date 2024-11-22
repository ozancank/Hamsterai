using Application.Features.Packages.Models.Packages;

namespace Application.Features.Lessons.Models.Lessons;

public sealed class GetLessonModel : IResponseModel
{
    public short Id { get; set; }
    public bool IsActive { get; set; }
    public long CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public long UpdateUser { get; set; }
    public DateTime UpdateDate { get; set; }
    public string? Name { get; set; }
    public short SortNo { get; set; }
    public byte AIUrlIndex { get; set; }
    public LessonTypes Type { get; set; }

    public List<GetPackageLiteModel> Packages { get; set; } = [];
}