namespace Business.Features.Users.Models.User;

public sealed class UpdateUserModel : IRequestModel
{
    public long Id { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Phone { get; set; }
    public string ProfileUrl { get; set; }
    public string Email { get; set; }
    public UserTypes Type { get; set; }
    public int? ConnectionId { get; set; }
    public int? SchoolId { get; set; }

    public string ProfilePictureBase64 { get; set; }
    public string ProfilePictureFileName { get; set; }
}