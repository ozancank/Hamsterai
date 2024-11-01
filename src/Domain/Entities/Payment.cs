using Domain.Constants;
using Domain.Entities.Core;

namespace Domain.Entities;

public class Payment : BaseEntity<int>
{
    public long UserId { get; set; }
    public double Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentReason PaymentReason { get; set; }
    public string? ReasonId { get; set; }
    public string? SipayMerchantKey { get; set; }
    public string? SipayPlanCode { get; set; }

    public virtual User? User { get; set; }

    public Payment() : base()
    { }

    public Payment(int id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    { }
}