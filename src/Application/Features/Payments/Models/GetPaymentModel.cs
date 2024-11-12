namespace Application.Features.Payments.Models;

public class GetPaymentModel : IResponseModel
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreateDate { get; set; }
    public long UserId { get; set; }
    public double Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentReason PaymentReason { get; set; }
    public string? ReasonId { get; set; }
    public string? SipayMerchantKey { get; set; }
    public string? SipayPlanCode { get; set; }
}