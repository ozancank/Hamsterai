using Domain.Entities.Core;
using OCK.Core.Security.JWT;

namespace Business.Services.AuthService;

public interface IAuthService : IBusinessService
{
    public Task<AccessToken> CreateAccessToken(User user);

    public Task<RefreshToken> CreateRefreshToken(User user, string ipAddress);

    public Task<RefreshToken> GetRefreshTokenByToken(string token);

    public Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken);

    public Task DeleteOldRefreshTokens(long userId);

    public Task RevokeDescendantRefreshTokens(RefreshToken refreshToken, string ipAddress, string reason);

    public Task<RefreshToken> RevokeRefreshToken(RefreshToken refreshToken, string ipAddress, string? reason = null, string? replacedByToken = null);

    public Task<RefreshToken> RotateRefreshToken(User user, RefreshToken refreshToken, string ipAddress);
}