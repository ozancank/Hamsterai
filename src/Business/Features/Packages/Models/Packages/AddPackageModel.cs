namespace Business.Features.Packages.Models.Packages;

public sealed class AddPackageModel : IResponseModel
{
    public string? Name { get; set; }
    public short SortNo { get; set; }
    public bool IsWebVisible { get; set; }
    public double TaxRatio { get; set; }
    public double Amount { get; set; }
    public double? OldAmount { get; set; }
    public PaymentRenewalPeriod PaymentRenewalPeriod { get; set; }
    public string? Description { get; set; }
    public IFormFile? PictureFile { get; set; }
    public byte CategoryId { get; set; }
    public int QuestionCredit { get; set; }

    public List<short> LessonIds { get; set; } = [];
}