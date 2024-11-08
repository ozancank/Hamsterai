
namespace Application.Features.Orders.Models;

public sealed class AddOrderModel : IRequestModel
{
    public AddUserForAddOrderModel? User { get; set; }
    public List<AddOrderDetailForAddOrderModel> OrderDetails { get; set; } = [];
    public AddPaymentModelForAddOrderModel? Payment { get; set; }
}

public sealed class AddUserForAddOrderModel : IRequestModel
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Phone { get; set; }        
    public string? TaxNumber { get; set; }
    public bool AutomaticPayment { get; set; }
}

public sealed class AddOrderDetailForAddOrderModel : IRequestModel
{
    public short PackageId { get; set; }
    public byte Quantity { get; set; }
    public double DiscountRatio { get; set; }
    public double Amount { get; set; }
}

public sealed class AddPaymentModelForAddOrderModel : IRequestModel
{
    public double Amount { get; set; }
    public string? SipayMerchantKey { get; set; }
    public string? SipayPlanCode { get; set; }
}