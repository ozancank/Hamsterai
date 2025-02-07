using Domain.Entities.Core;

namespace Domain.Entities;

public class PaymentSipay : BaseEntity<Guid>
{
    public long UserId { get; set; }
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
    public string? Amount { get; set; }
    public string? PaymentReasonCode { get; set; }
    public string? PaymentReasonCodeDetail { get; set; }
    public string? HashKey { get; set; }
    public string? MdStatus { get; set; }
    public string? OriginalBankErrorCode { get; set; }
    public string? OriginalBankErrorDescription { get; set; }
    public string? BankStatusCode { get; set; }
    public string? BankStatusDescription { get; set; }
    public string? TransactionStatus { get; set; }
    public string? TransactionId { get; set; }
    public string? Message { get; set; }
    public string? Reason { get; set; }
    public int TotalRefundedAmount { get; set; }
    public string? ProductPrice { get; set; }
    public int TransactionAmount { get; set; }
    public int RecurringId { get; set; }
    public string? RefNumber { get; set; }
    public string? RecurringPlanCode { get; set; }
    public int RecurringNumber { get; set; }
    public string? ActionDate { get; set; }
    public string? NextActionDate { get; set; }
    public string? RecurringStatus { get; set; }
    public string? SettlementDate { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = [];

    public PaymentSipay() : base()
    { }

    public PaymentSipay(Guid id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    { }
}