using Business.Features.Auths.Rules;
using Business.Services.AuthService;
using MediatR;

namespace Business.Features.Auths.Commands.RefreshTokens;

public class RevokeTokenCommand : IRequest<bool>
{
    public string RefreshToken { get; set; }
    public string IpAddress { get; set; }
}

public class RevokeTokenCommandHandler(IAuthService authService) : IRequestHandler<RevokeTokenCommand, bool>
{
    public async Task<bool> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        if (request.RefreshToken == null) return false;

        var refreshToken = await authService.GetRefreshTokenByToken(request.RefreshToken!);
        await AuthRules.RefreshTokenShouldBeExists(refreshToken);
        await AuthRules.RefreshTokenShouldBeActive(refreshToken!);
        var revokedRefreshToken = await authService.RevokeRefreshToken(refreshToken!, request.IpAddress!, Strings.RevokeRefreshTokenLogout);
        return revokedRefreshToken.Revoked != null;
    }
}