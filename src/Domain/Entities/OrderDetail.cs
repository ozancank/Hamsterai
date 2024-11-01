using Domain.Entities.Core;

namespace Domain.Entities;

public class OrderDetail : BaseEntity<Guid>
{
    public int OrderId { get; set; }
    public byte PackageId { get; set; }
    public int QuestionCredit { get; set; }
    public byte Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double DiscountRatio { get; set; }
    public double DiscountAmount { get; set; }
    public double TaxBase { get; set; }
    public double TaxRatio { get; set; }
    public double TaxAmount { get; set; }
    public double Amount { get; set; }

    public virtual Order? Order { get; set; }
    public virtual Package? Package { get; set; }

    public OrderDetail() : base()
    { }

    public OrderDetail(Guid id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    { }
}