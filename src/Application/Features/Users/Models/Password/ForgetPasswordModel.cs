namespace Application.Features.Users.Models.Password;

public class ForgetPasswordModel : IRequestModel
{
    public string? Email { get; set; }
}