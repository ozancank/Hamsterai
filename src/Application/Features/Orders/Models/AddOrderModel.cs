using Application.Features.Orders.Constants;

namespace Application.Features.Orders.Models;

public sealed class AddOrderModel : IRequestModel
{
    public string? Email { get; set; }

    //public AddUserForAddOrderModel? User { get; set; }
    public List<AddOrderDetailForAddOrderModel> OrderDetails { get; set; } = [];

    public AddPaymentSipayModelForAddOrderModel? Payment { get; set; }
}

//public sealed class AddUserForAddOrderModel : IRequestModel
//{
//    public string? Name { get; set; }
//    public string? Surname { get; set; }
//    public string? Email { get; set; }
//    public string? Password { get; set; }
//    public string? Phone { get; set; }
//    public string? TaxNumber { get; set; }
//    public bool AutomaticPayment { get; set; }
//}

public sealed class AddOrderDetailForAddOrderModel : IRequestModel
{
    public short PackageId { get; set; }
    public byte Quantity { get; set; }
    public double DiscountRatio { get; set; }
    public double Amount { get; set; }
}

public sealed class AddPaymentSipayModelForAddOrderModel : IRequestModel
{
    public AddPaymentSipayModelForAddOrderModel()
    {
        OrderStatics.AddPaymentSipayModelForAddOrderModelStringProperties ??=
            GetType().GetProperties().Where(p => p.PropertyType == typeof(string)).ToArray();
    }

    public double Amount { get; set; }
    public string? Status { get; set; }
    public string? OrderNo { get; set; }
    public string? OrderId { get; set; }
    public string? InvoiceId { get; set; }
    public string? StatusCode { get; set; }
    public string? StatusDescription { get; set; }
    public string? PaymentMethod { get; set; }
    public string? CreditCardNo { get; set; }
    public string? TransactionType { get; set; }
    public string? PaymentStatus { get; set; }
    public string? PaymentMethodCode { get; set; }
    public string? ErrorCode { get; set; }
    public string? Error { get; set; }
    public string? AuthCode { get; set; }
    public string? MerchantCommission { get; set; }
    public string? UserCommission { get; set; }
    public string? MerchantCommissionPercentage { get; set; }
    public string? MerchantCommissionFixed { get; set; }
    public string? Installment { get; set; }
    public string? PaymentReasonCode { get; set; }
    public string? PaymentReasonCodeDetail { get; set; }
    public string? HashKey { get; set; }
    public string? MdStatus { get; set; }
    public string? OriginalBankErrorCode { get; set; }
    public string? OriginalBankErrorDescription { get; set; }
}