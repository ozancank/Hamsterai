namespace Business.Features.Packages.Models;

public sealed class AddLessonInPackageModel : IResponseModel
{
    public byte PackageId { get; set; }
    public List<byte> LessonIds { get; set; } = [];
}