namespace Business.Features.Packages.Models;

public sealed class UpdatePackageModel : IResponseModel
{
    public byte Id { get; set; }
    public string? Name { get; set; }
    public List<byte> LessonIds { get; set; } = [];
}