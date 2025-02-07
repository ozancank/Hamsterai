using Application.Features.Auths.Rules;
using Application.Services.AuthService;
using MediatR;

namespace Application.Features.Auths.Commands.RefreshTokens;

public class RevokeTokenCommand : IRequest<bool>
{
    public required string RefreshToken { get; set; }
    public required string IpAddress { get; set; }
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