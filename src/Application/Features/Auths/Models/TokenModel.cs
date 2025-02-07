using Application.Features.Users.Models.User;

namespace Application.Features.Auths.Models;

public sealed class TokenModel : IResponseModel
{
    public AccessTokenModel? AccessToken { get; set; }
    public RefreshTokenModel? RefreshToken { get; set; }
    public GetUserModel? UserInfo { get; set; }
}

public sealed class RefreshTokenModel : IResponseModel
{
    public string? Token { get; set; }
    public DateTime Expiration { get; set; }
}

public sealed class AccessTokenModel : IResponseModel
{
    public string? Token { get; set; }
    public DateTime Expiration { get; set; }
}