namespace Business.Features.Auths.Models;

public sealed class LoginModel : IRequestModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public byte AuthenticatorCode { get; set; }
}