using Application.Features.Packages.Models.Packages;

namespace Application.Features.Packages.Models.PackageCategories;

public sealed class GetPackageCategoryModel : IResponseModel
{
    public byte Id { get; set; }
    public bool IsActive { get; set; }
    public long CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public long UpdateUser { get; set; }
    public DateTime UpdateDate { get; set; }
    public string? Name { get; set; }
    public byte SortNo { get; set; }
    public bool IsWebVisible { get; set; }
    public byte ParentId { get; set; }
    public string? Description { get; set; }
    public string? PictureUrl { get; set; }
    public string? Slug { get; set; }

    public GetPackageCategoryLiteModel? TopCategory { get; set; }
    public List<byte> SubCategoryIds { get; set; } = [];
    public List<GetPackageCategoryLiteModel> SubCategories { get; set; } = [];
    public List<short> PackageIds { get; set; } = [];
    public List<GetPackageLiteModel> Packages { get; set; } = [];
}