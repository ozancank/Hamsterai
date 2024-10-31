namespace Business.Features.Packages.Models;

public sealed class AddPackageModel : IResponseModel
{
    public string? Name { get; set; }
    public List<byte> PackageIds { get; set; } = [];
    public List<byte> LessonIds { get; set; } = [];
}