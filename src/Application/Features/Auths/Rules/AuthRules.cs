using DataAccess.Abstract.Core;
using Domain.Entities.Core;
using OCK.Core.Security.HashingHelper;

namespace Application.Features.Auths.Rules;

public class AuthRules(IUserDal userDal) : IBusinessRule
{
    internal static Task UserShouldExistsByUserNameWhenLogin(User user)
    {
        if (user == null) throw new AuthenticationException(Strings.UserOrPasswordNotWrong);
        return Task.CompletedTask;
    }

    internal async Task UserShouldExistsByUserNameWhenLogin(string userName)
    {
        var control = await userDal.IsExistsAsync(predicate: x => x.UserName == userName);
        if (!control) throw new BusinessException(Strings.UserOrPasswordNotWrong);
    }

    internal static Task PasswordShouldVerifiedWhenLogin(User user, string password)
    {
        var control = HashingHelper.VerifyPasswordHash(password, user.PasswordHash!, user.PasswordSalt!);
        if (!control) throw new AuthenticationException(Strings.UserOrPasswordNotWrong);
        return Task.CompletedTask;
    }

    internal static Task UserShouldExistsAndActiveByUserNameWhenLogin(User user)
    {
        if (!user.IsActive) throw new AuthenticationException(Strings.UserIsPassive);
        return Task.CompletedTask;
    }

    internal static Task RefreshTokenShouldBeExists(RefreshToken refreshToken)
    {
        if (refreshToken == null) throw new BusinessException(Strings.SessionTerminated);
        return Task.CompletedTask;
    }

    internal static Task RefreshTokenShouldBeActive(RefreshToken refreshToken)
    {
        if (refreshToken.Revoked != null || refreshToken.Expires < DateTime.UtcNow)
            throw new BusinessException(Strings.SessionTerminated);
        return Task.CompletedTask;
    }
}