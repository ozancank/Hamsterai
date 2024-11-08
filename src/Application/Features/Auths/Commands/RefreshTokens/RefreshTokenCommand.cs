using Application.Features.Auths.Models;
using Application.Features.Auths.Rules;
using Application.Features.Users.Models.User;
using Application.Features.Users.Rules;
using Application.Services.AuthService;
using MediatR;

namespace Application.Features.Auths.Commands.RefreshTokens;

public class RefreshTokenCommand : IRequest<TokenModel>
{
    public required string RefreshToken { get; set; }
    public required string IpAddress { get; set; }
}

public class RefreshTokenCommandHandler(IAuthService authService,
                                        IMapper mapper) : IRequestHandler<RefreshTokenCommand, TokenModel>
{
    public async Task<TokenModel> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (request.RefreshToken == null) return new TokenModel();

        var refreshToken = await authService.GetRefreshTokenByToken(request.RefreshToken);
        await AuthRules.RefreshTokenShouldBeExists(refreshToken);

        if (refreshToken!.Revoked != null)
            await authService.RevokeDescendantRefreshTokens(refreshToken, request.IpAddress!, Strings.InvalidRefreshTokenReuse);
        await AuthRules.RefreshTokenShouldBeActive(refreshToken);
        await UserRules.UserShouldExistsAndActive(refreshToken.User);

        var newRefreshToken = await authService.RotateRefreshToken(refreshToken.User!, refreshToken, request.IpAddress!);
        var addedRefreshToken = await authService.AddRefreshToken(newRefreshToken) ?? throw new AuthenticationException();

        await authService.DeleteOldRefreshTokens(refreshToken.UserId);

        var createdAccessToken = await authService.CreateAccessToken(refreshToken.User!);

        var result = new TokenModel
        {
            AccessToken = mapper.Map<AccessTokenModel>(createdAccessToken),
            RefreshToken = mapper.Map<RefreshTokenModel>(addedRefreshToken),
            UserInfo = mapper.Map<GetUserModel>(refreshToken.User)
        };
        return result;
    }
}