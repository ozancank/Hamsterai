namespace Application.Features.Users.Models.User;

public sealed class AddUserModel : IRequestModel
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Phone { get; set; }
    public string? ProfileUrl { get; set; }
    public UserTypes Type { get; set; }
    public int? ConnectionId { get; set; }
    public int? SchoolId { get; set; }
    public List<short> PackageIds { get; set; } = [];
    public bool AutomaticPayment { get; set; }
    public string? TaxNumber { get; set; }
    public DateTime LicenceEndDate { get; set; }
    public int QuestionCredit { get; set; }
    public string? ExitPassword { get; set; }

    public string? ProfilePictureBase64 { get; set; }
    public string? ProfilePictureFileName { get; set; }
}