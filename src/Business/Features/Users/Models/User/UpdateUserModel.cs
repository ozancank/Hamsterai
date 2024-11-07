namespace Business.Features.Users.Models.User;

public sealed class UpdateUserModel : IRequestModel
{
    public long Id { get; set; }
    public required string UserName { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Phone { get; set; }
    public string? ProfileUrl { get; set; }
    public required string Email { get; set; }
    public UserTypes Type { get; set; }
    public int? ConnectionId { get; set; }
    public int? SchoolId { get; set; }
    public short PackageId { get; set; }
    public int PackageCredit { get; set; }
    public int AddtionalCredit { get; set; }
    public bool AutomaticPayment { get; set; }

    public string? ProfilePictureBase64 { get; set; }
    public string? ProfilePictureFileName { get; set; }
}