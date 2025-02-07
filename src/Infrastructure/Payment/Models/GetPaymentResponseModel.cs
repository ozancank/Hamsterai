using OCK.Core.Interfaces;

namespace Infrastructure.Payment.Models;

public sealed class GetPaymentResponseModel : IResponseModel
{
    public int StatusCode { get; set; }
    public string? StatusDescription { get; set; }
    public string? TransactionStatus { get; set; }
    public string? OrderId { get; set; }
    public string? TransactionId { get; set; }
    public string? Message { get; set; }
    public string? Reason { get; set; }
    public string? BankStatusCode { get; set; }
    public string? BankStatusDescription { get; set; }
    public string? InvoiceId { get; set; }
    public int TotalRefundedAmount { get; set; }
    public string? ProductPrice { get; set; }
    public int TransactionAmount { get; set; }
    public int RecurringId { get; set; }
    public string? RefNumber { get; set; }
    public string? RecurringPlanCode { get; set; }
    public string? NextActionDate { get; set; }
    public string? RecurringStatus { get; set; }
    public string? MerchantCommission { get; set; }
    public string? UserCommission { get; set; }
    public string? SettlementDate { get; set; }
    public int MdStatus { get; set; }
    public int Installment { get; set; }
}