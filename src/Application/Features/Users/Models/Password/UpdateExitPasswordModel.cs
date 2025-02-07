namespace Application.Features.Users.Models.Password;

public sealed class UpdateExitPasswordModel : IRequestModel
{
    public long Id { get; set; }
    public string? OldPassword { get; set; }
    public string? Password { get; set; }
}