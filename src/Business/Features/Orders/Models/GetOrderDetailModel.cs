using Business.Features.Packages.Models.Packages;

namespace Business.Features.Orders.Models;

public class GetOrderDetailModel : IResponseModel
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
    public long CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public long UpdateUser { get; set; }
    public int OrderId { get; set; }
    public short PackageId { get; set; }
    public int QuestionCredit { get; set; }
    public byte Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double DiscountRatio { get; set; }
    public double DiscountAmount { get; set; }
    public double TaxBase { get; set; }
    public double TaxRatio { get; set; }
    public double TaxAmount { get; set; }
    public double Amount { get; set; }

    public GetPackageModel? Package { get; set; }
}