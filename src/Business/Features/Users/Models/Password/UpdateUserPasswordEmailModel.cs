namespace Business.Features.Users.Models.Password;

public class UpdateUserPasswordEmailModel : IRequestModel
{
    public string Password { get; set; }
    public string Token { get; set; }
}