using Business.Features.Users.Models.User;

namespace Business.Features.Orders.Models;

public class GetOrderModel : IResponseModel
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public long CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public long UpdateUser { get; set; }
    public DateTime UpdateDate { get; set; }
    public long UserId { get; set; }
    public string? OrderNo { get; set; }
    public int QuestionCredit { get; set; }
    public double SubTotal { get; set; }
    public double DiscountAmount { get; set; }
    public double TaxBase { get; set; }
    public double TaxAmount { get; set; }
    public double Amount { get; set; }
    public GetUserModel? User { get; set; }
    public List<GetOrderDetailModel> Details { get; set; } = [];    
}