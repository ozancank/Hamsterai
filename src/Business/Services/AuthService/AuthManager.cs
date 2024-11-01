using DataAccess.Abstract.Core;
using Domain.Entities.Core;
using OCK.Core.Security.JWT;
using System.Security.Claims;
using Security = OCK.Core.Security.Entities;

namespace Business.Services.AuthService;

public class AuthManager(IConfiguration configuration,
                         ITokenHelper tokenHelper,
                         IRefreshTokenDal refreshTokenDal) : IAuthService
{
    private readonly TokenOptions _tokenOptions = configuration.GetSection("TokenOptions").Get<TokenOptions>()!;

    public Task<AccessToken> CreateAccessToken(User user)
    {
        var operationClaims = user.UserOperationClaims.Select(x => new Security.OperationClaim
        {
            Id = x.OperationClaim!.Id,
            Name = x.OperationClaim.Name,
            Description = x.OperationClaim.Description
        }).ToList();

        user.Type = user.Type == 0 ? UserTypes.Student : user.Type;

        List<Claim> claims = [
                new(Domain.Constants.ClaimTypes.UserType, $"{(int)user.Type}", ClaimValueTypes.Integer),
                new(Domain.Constants.ClaimTypes.SchoolId, $"{user.SchoolId}", ClaimValueTypes.String),
                new(Domain.Constants.ClaimTypes.ConnectionId, $"{user.ConnectionId}", ClaimValueTypes.String),
                new(Domain.Constants.ClaimTypes.PackageId, $"{user.PackageUsers}", ClaimValueTypes.String)
            ];

        var accessToken = tokenHelper.CreateToken(user, operationClaims, $"{user.Name} {user.Surname}", user.Email, claims);
        return Task.FromResult(accessToken);
    }

    public async Task<RefreshToken> CreateRefreshToken(User user, string ipAddress)
    {
        var refreshToken = new RefreshToken();
        tokenHelper.CreateRefreshToken(refreshToken, user, ipAddress);
        return await Task.FromResult(refreshToken);
    }

    public async Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken)
    {
        refreshToken.Id = await refreshTokenDal.GetNextPrimaryKeyAsync(x => x.Id);
        var addedRefreshToken = await refreshTokenDal.AddAsyncCallback(refreshToken);
        return addedRefreshToken;
    }

    public async Task<RefreshToken> GetRefreshTokenByToken(string token)
    {
        var refreshToken = await refreshTokenDal.GetAsync(
            predicate: x => x.Token == token,
            include: x => x.Include(u => u.User));
        return refreshToken;
    }

    public async Task DeleteOldRefreshTokens(long userId)
    {
        var refreshTokens = await refreshTokenDal.GetListAsync(
            predicate: x => x.UserId == userId
                         && (x.Revoked != null || x.Expires < DateTime.Now)
                         && x.Created.AddDays(_tokenOptions.RefreshTokenTTL) <= DateTime.Now);

        foreach (var refreshToken in refreshTokens) await refreshTokenDal.DeleteAsync(refreshToken);
    }

    public async Task RevokeDescendantRefreshTokens(RefreshToken refreshToken, string ipAddress, string reason)
    {
        if (string.IsNullOrEmpty(refreshToken.ReplacedByToken)) return;

        var childToken = await refreshTokenDal.GetAsync(x => x.Token == refreshToken.ReplacedByToken);
        if (childToken == null) return;

        if (childToken.Revoked == null && childToken.Expires >= DateTime.UtcNow) await RevokeRefreshToken(childToken, ipAddress, reason);
        else await RevokeDescendantRefreshTokens(childToken, ipAddress, reason);
    }

    public async Task<RefreshToken> RevokeRefreshToken(RefreshToken refreshToken, string ipAddress, string? reason = null, string? replacedByToken = null)
    {
        refreshToken.Revoked = DateTime.UtcNow;
        refreshToken.RevokedByIp = ipAddress;
        refreshToken.ReasonRevoked = reason;
        refreshToken.ReplacedByToken = replacedByToken;
        var updatedRefreshToken = await refreshTokenDal.UpdateAsyncCallback(refreshToken);
        return updatedRefreshToken;
    }

    public async Task<RefreshToken> RotateRefreshToken(User user, RefreshToken refreshToken, string ipAddress)
    {
        var newRefreshToken = new RefreshToken();
        tokenHelper.CreateRefreshToken(newRefreshToken, user, ipAddress);
        await RevokeRefreshToken(refreshToken, ipAddress, Strings.ReplacedByNewToken, newRefreshToken.Token);
        return newRefreshToken;
    }
}