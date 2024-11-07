namespace Business.Features.Users.Models.User;

public class GetUserModel : IResponseModel
{
    public long Id { get; set; }
    public bool IsActive { get; set; }
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Phone { get; set; }
    public string? ProfileFileName { get; set; }
    public string? Email { get; set; }
    public string? FullName => $"{Name} {Surname}";
    public int? ConnectionId { get; set; }
    public int? SchoolId { get; set; }
    public byte Type { get; set; }
    public short PackageId { get; set; }
    public int PackageCredit { get; set; }
    public int AddtionalCredit { get; set; }
    public bool AutomaticPayment { get; set; }

    public List<string> OperationClaims { get; set; } = [];
}