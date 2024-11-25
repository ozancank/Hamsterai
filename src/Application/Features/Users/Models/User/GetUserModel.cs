using Application.Features.Packages.Models.Packages;

namespace Application.Features.Users.Models.User;

public class GetUserModel : IResponseModel
{
    public long Id { get; set; }
    public bool IsActive { get; set; }
    public bool MustPasswordChange { get; set; }
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
    public int TotalCredit { get; set; }
    public int QuestionCredit { get; set; }
    public int RemainingCredit => TotalCredit - QuestionCredit;
    public bool AutomaticPayment { get; set; }
    public string? TaxNumber { get; set; }
    public DateTime LicenceEndDate { get; set; }

    public List<GetPackageModel> Packages { get; set; } = [];
    public List<string> OperationClaims { get; set; } = [];
}