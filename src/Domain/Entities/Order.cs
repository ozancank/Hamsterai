using Domain.Entities.Core;

namespace Domain.Entities;

public class Order : BaseEntity<int>
{
    public long UserId { get; set; }
    public string? OrderNo { get; set; }
    public int QuestionCredit { get; set; }
    public double SubTotal { get; set; }
    public double DiscountAmount { get; set; }
    public double TaxBase { get; set; }
    public double TaxAmount { get; set; }
    public double Amount { get; set; }

    public virtual User? User { get; set; }
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = [];

    public Order()
    { }

    public Order(int id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    { }
}