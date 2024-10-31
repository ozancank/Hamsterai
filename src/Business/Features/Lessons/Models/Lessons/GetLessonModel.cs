using Business.Features.Packages.Models;

namespace Business.Features.Lessons.Models.Lessons;

public sealed class GetLessonModel : IResponseModel
{
    public byte Id { get; set; }
    public bool IsActive { get; set; }
    public long CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public long UpdateUser { get; set; }
    public DateTime UpdateDate { get; set; }
    public string? Name { get; set; }
    public byte SortNo { get; set; }

    public List<GetPackageLiteModel> Packages { get; set; } = [];
}