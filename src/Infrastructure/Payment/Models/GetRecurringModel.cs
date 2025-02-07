using OCK.Core.Interfaces;

namespace Infrastructure.Payment.Models;

public sealed class GetRecurringModel : IResponseModel
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public int RecurringId { get; set; }
    public double FirstAmount { get; set; }
    public double RecurringAmount { get; set; }
    public double TotalAmount { get; set; }
    public int PaymentNumber { get; set; }
    public int PaymentInterval { get; set; }
    public string? PaymentCycle { get; set; }
    public string? FirstOrderId { get; set; }
    public int MerchantId { get; set; }
    public string? CardNo { get; set; }
    public string? NextActionDate { get; set; }
    public string? RecurringStatus { get; set; }
    public string? TransactionDate { get; set; }
    public List<GetRecurringTransactionModel> TransactionHistories { get; set; } = [];
}


public sealed class GetRecurringTransactionModel
{
    public int Id { get; set; }
    public int SaleRecurringId { get; set; }
    public int SaleId { get; set; }
    public int MerchantId { get; set; }
    public int SaleRecurringPaymentScheduleId { get; set; }
    public double Amount { get; set; }
    public string? ActionDate { get; set; }
    public string? Status { get; set; }
    public int RecurringNumber { get; set; }
    public int Attempts { get; set; }
    public string? Remarks { get; set; }
}

