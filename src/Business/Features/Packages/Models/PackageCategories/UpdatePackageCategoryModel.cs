namespace Business.Features.Packages.Models.PackageCategories;

public sealed class UpdatePackageCategoryModel : IResponseModel
{
    public byte Id { get; set; }
    public string? Name { get; set; }
    public byte SortNo { get; set; }
    public bool IsWebVisible { get; set; }
    public byte ParentId { get; set; }
    public string? Description { get; set; }
    public IFormFile? PictureFile { get; set; }
}