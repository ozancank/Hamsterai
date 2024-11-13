namespace Application.Features.Packages.Models.Packages;

public sealed class GetPackageLiteModel : IResponseModel
{
    public short Id { get; set; }
    public string? Name { get; set; }
    public short SortNo { get; set; }
    public bool IsWebVisible { get; set; }
    public double Amount { get; set; }
    public double? OldAmount { get; set; }
    public PaymentRenewalPeriod PaymentRenewalPeriod { get; set; }
    public string? Description { get; set; }
    public string? PictureUrl { get; set; }
    public string? Slug { get; set; }
    public int QuestionCredit { get; set; }
    public byte? CategoryId { get; set; }
    public PackageType Type { get; set; }

    public List<short> LessonIds { get; set; } = [];
}