namespace Business.Features.Users.Models.Password;

public sealed class UpdateUserPasswordModel : IRequestModel
{
    public long Id { get; set; }
    public string OldPassword { get; set; }
    public string Password { get; set; }
}