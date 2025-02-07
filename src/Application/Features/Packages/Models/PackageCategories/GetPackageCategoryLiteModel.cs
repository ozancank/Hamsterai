namespace Application.Features.Packages.Models.PackageCategories;

public sealed class GetPackageCategoryLiteModel : IResponseModel
{
    public byte Id { get; set; }
    public bool IsActive { get; set; }
    public bool IsWebVisible { get; set; }
    public string? Name { get; set; }
    public byte SortNo { get; set; }
    public byte? ParentId { get; set; }
    public string? Description { get; set; }
    public string? PictureUrl { get; set; }
    public string? Slug { get; set; }
}