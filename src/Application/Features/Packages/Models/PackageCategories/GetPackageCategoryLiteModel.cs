namespace Application.Features.Packages.Models.PackageCategories;

public sealed class GetPackageCategoryLiteModel : IResponseModel
{
    public byte Id { get; set; }
    public string? Name { get; set; }
    public byte SortNo { get; set; }
    public byte? ParentId { get; set; }
    public string? Slug { get; set; }
}