using Domain.Constants;
using Domain.Entities.Core;

namespace Domain.Entities;

public class Payment : BaseEntity<Guid>
{
    public long UserId { get; set; }
    public double Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentReason PaymentReason { get; set; }
    public string? ReasonId { get; set; }
    public Guid? PaymentSipayId { get; set; }

    public virtual User? User { get; set; }
    public virtual PaymentSipay? PaymentSipay { get; set; }

    public Payment() : base()
    { }

    public Payment(Guid id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    { }
}