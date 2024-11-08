using Application.Features.Users.Models.User;
using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using Domain.Entities.Core;
using OCK.Core.Security.HashingHelper;

namespace Application.Features.Users.Rules;

public class UserRules(IUserDal userDal,
                       IUserOperationClaimDal userOperationClaimDal,
                       ICommonService commonService) : IBusinessRule
{
    internal static Task UserShouldExists(User? user)
    {
        if (user == null) throw new BusinessException(Strings.DynamicNotFound, Strings.User);
        return Task.CompletedTask;
    }

    internal static Task UserShouldExistsAndActive(User? user)
    {
        UserShouldExists(user);
        if (!user!.IsActive) throw new BusinessException(Strings.DynamicNotFoundOrActive, Strings.User);
        return Task.CompletedTask;
    }

    internal static Task UserShouldExists(GetUserModel? user)
    {
        if (user == null) throw new BusinessException(Strings.DynamicNotFound, Strings.User);
        return Task.CompletedTask;
    }

    internal static Task UserShouldExistsAndActive(GetUserModel? user)
    {
        UserShouldExists(user);
        if (!user!.IsActive) throw new BusinessException(Strings.DynamicNotFoundOrActive, Strings.User);
        return Task.CompletedTask;
    }

    internal async Task UserShouldExistsAndActiveById(long id)
    {
        var control = await userDal.IsExistsAsync(predicate: x => x.Id == id && x.IsActive, enableTracking: false);
        if (!control) throw new BusinessException(Strings.DynamicNotFoundOrActive, Strings.User);
    }

    internal async Task UserShouldNotExistsById(long id)
    {
        var control = await userDal.IsExistsAsync(predicate: x => x.Id == id, enableTracking: false);
        if (control) throw new BusinessException(Strings.DynamicExists, Strings.User);
    }

    internal async Task UserNameCanNotBeDuplicated(string userName, long? userId = null)
    {
        userName = userName.Trim().ToLower();
        var user = await userDal.GetAsync(predicate: x => x.UserName == userName, enableTracking: false);
        if (userId == null && user != null) throw new BusinessException(Strings.DynamicExists, Strings.UserName);
        if (userId != null && user != null && user.Id != userId) throw new BusinessException(Strings.DynamicExists, Strings.UserName);
    }

    internal async Task UserEmailCanNotBeDuplicated(string email, long? userId = null)
    {
        email = email.Trim().ToLower();
        var user = await userDal.GetAsync(predicate: x => x.Email == email, enableTracking: false);
        if (userId == null && user != null) throw new BusinessException(Strings.DynamicExists, Strings.Email);
        if (userId != null && user != null && user.Id != userId) throw new BusinessException(Strings.DynamicExists, Strings.Email);
    }

    internal async Task UserPhoneCanNotBeDuplicated(string phone, long? userId = null)
    {
        phone = phone.TrimForPhone();
        var user = await userDal.GetAsync(predicate: x => x.Phone == phone, enableTracking: false);
        if (userId == null && user != null) throw new BusinessException(Strings.DynamicExists, Strings.Phone);
        if (userId != null && user != null && user.Id != userId) throw new BusinessException(Strings.DynamicExists, Strings.Phone);
    }

    internal async Task UserShouldExistsById(long id)
    {
        var control = await userDal.IsExistsAsync(predicate: x => x.Id == id, enableTracking: false);
        if (!control) throw new BusinessException(Strings.DynamicNotFound, Strings.User);
    }

    internal async Task UserCanNotEditAtAdminUser(long id)
    {
        var user = await userDal.GetAsync(
            predicate: x => x.Id == id,
            enableTracking: false,
            include: x => x.Include(u => u.UserOperationClaims).ThenInclude(u => u.OperationClaim!));
        var isAdmin = user!.UserOperationClaims.Select(x => x.OperationClaim!.Name).Contains(OperationClaims.Admin);
        if (isAdmin) throw new BusinessException(Strings.UserDeniedEditForAdmin);
    }

    internal static Task AdminCanNotAssignClaim(IList<string> assignRoles)
    {
        var control = assignRoles.Contains(OperationClaims.Admin, StringComparer.OrdinalIgnoreCase);
        if (control) throw new BusinessException(Strings.UserAdminClaimNotAssign);
        return Task.CompletedTask;
    }

    internal static Task AssignRolesShouldBeRecordInDatabase(IList<string> assignRoles, IEnumerable<OperationClaim> operationClaims)
    {
        foreach (var role in assignRoles)
            if (!operationClaims.Any(x => x.Name == role))
                throw new BusinessException(Strings.UserWrongRoles);
        return Task.CompletedTask;
    }

    internal static Task PictureShouldAllowedType(string profilePictureFileName)
    {
        string[] allowedExtensions = [".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".webp"];
        var extension = Path.GetExtension(profilePictureFileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension)) throw new BusinessException(Strings.UnsupportedFileFormat);
        return Task.CompletedTask;
    }

    internal static Task PasswordTokenShouldExists(PasswordToken passwordToken)
    {
        if (passwordToken is null) throw new BusinessException(Strings.PasswordCodeExpired);
        return Task.CompletedTask;
    }

    internal static Task IsPasswordTokenExpried(DateTime expried)
    {
        if (expried < DateTime.Now) throw new BusinessException(Strings.PasswordCodeExpired);
        return Task.CompletedTask;
    }

    internal Task UserTypeAllowed(UserTypes type, long? userId = null)
    {
        var control = commonService.HttpUserType switch
        {
            UserTypes.Administator => true,
            UserTypes.School => type == UserTypes.Teacher || type == UserTypes.Student,
            UserTypes.Teacher => type == UserTypes.Student || (userId.HasValue && commonService.HttpUserId == userId && type == UserTypes.Teacher),
            UserTypes.Student => userId.HasValue && commonService.HttpUserId == userId && type == UserTypes.Student,
            UserTypes.Person => userId.HasValue && commonService.HttpUserId == userId && type == UserTypes.Student,
            _ => false
        };

        if (!control) throw new BusinessException(Strings.UserTypeNotAllowed);
        return Task.CompletedTask;
    }

    internal async Task UserCanNotPassiveAtAdminUser(User user)
    {
        if (user.Id is 1 or 2) throw new BusinessException(Strings.UserDeniedPassiveForAdmin);
        if (user.UserOperationClaims.Select(x => x.OperationClaim!.Name).Contains(OperationClaims.Admin)) throw new BusinessException(Strings.UserDeniedPassiveForAdmin);
        if (await userOperationClaimDal.IsExistsAsync(predicate: x => x.UserId == user.Id && x.OperationClaimId == 1)) throw new BusinessException(Strings.UserDeniedPassiveForAdmin);
    }

    internal static Task PasswordShouldVerifiedWhenPasswordChange(User user, string oldPassword)
    {
        var control = HashingHelper.VerifyPasswordHash(oldPassword, user.PasswordHash, user.PasswordSalt);
        if (!control) throw new AuthenticationException(Strings.OldPasswordWrong);
        return Task.CompletedTask;
    }

    internal static async Task LicenceIsValid(DateTime licenceDate)
    {
        if (licenceDate.Date <= DateTime.Now.Date) throw new BusinessException(Strings.LicenceExpired);
        await Task.CompletedTask;
    }

    internal Task UserCanNotChangeOwnOrAdminPassword(long userId)
    {
        if (userId is 1 or 2) throw new BusinessException(Strings.UserDeniedChangeOwnPassword);
        if (userId == commonService.HttpUserId) throw new BusinessException(Strings.UserDeniedChangeOwnPassword);
        return Task.CompletedTask;
    }
}