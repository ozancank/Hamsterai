using Business.Features.Lessons.Models.Lessons;
using Business.Features.Packages.Models.PackageCategories;

namespace Business.Features.Packages.Models.Packages;

public sealed class GetPackageModel : IResponseModel
{
    public short Id { get; set; }
    public bool IsActive { get; set; }
    public long CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public long UpdateUser { get; set; }
    public DateTime UpdateDate { get; set; }
    public string? Name { get; set; }
    public short SortNo { get; set; }
    public bool IsWebVisible { get; set; }
    public double UnitPrice { get; set; }
    public double? UnitOldPrice { get; set; }
    public double TaxRatio { get; set; }
    public double TaxAmount { get; set; }
    public double? TaxOldAmount { get; set; }
    public double Amount { get; set; }
    public double? OldAmount { get; set; }
    public PaymentRenewalPeriod PaymentRenewalPeriod { get; set; }
    public string? Description { get; set; }
    public string? PictureUrl { get; set; }
    public string? Slug { get; set; }
    public byte? CategoryId { get; set; }

    public GetPackageCategoryLiteModel? Category { get; set; }
    public List<short> LessonIds { get; set; } = [];
    public List<GetLessonLiteModel> Lessons { get; set; } = [];
}