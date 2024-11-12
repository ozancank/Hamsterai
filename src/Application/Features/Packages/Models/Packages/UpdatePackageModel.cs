namespace Application.Features.Packages.Models.Packages;

public sealed class UpdatePackageModel : IResponseModel
{
    public short Id { get; set; }
    public string? Name { get; set; }
    public short SortNo { get; set; }
    public bool IsWebVisible { get; set; }
    public double TaxRatio { get; set; }
    public double Amount { get; set; }
    public double? OldAmount { get; set; }
    public PaymentRenewalPeriod PaymentRenewalPeriod { get; set; }
    public string? Description { get; set; }
    public byte CategoryId { get; set; }
    public IFormFile? PictureFile { get; set; }
    public PackageType Type { get; set; }
    public int QuestionCredit { get; set; }

    public List<byte> LessonIds { get; set; } = [];
}