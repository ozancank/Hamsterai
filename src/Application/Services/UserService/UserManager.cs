using Application.Features.Schools.Rules;
using Application.Features.Users.Rules;
using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using DataAccess.EF.Migrations;
using Domain.Entities.Core;
using LinqKit;
using OCK.Core.Caching;
using System.Linq.Expressions;

namespace Application.Services.UserService;

public class UserManager(IUserDal userDal,
                         IMapper mapper,
                         ICacheManager cacheManager,
                         ICommonService commonService) : IUserService
{
    public async Task<User> GetUserById(long id, bool tracking = false)
    {
        var user = await userDal.GetAsync(
            enableTracking: tracking,
            predicate: GetPredicateForUser(x => x.Id == id),
            include: x => x.Include(u => u.UserOperationClaims).ThenInclude(u => u.OperationClaim));

        return user;
    }

    public async Task<T> GetUserById<T>(long id, bool tracking = false)
       where T : class, new()
    {
        var user = await userDal.GetAsyncAutoMapper<T>(
         enableTracking: tracking,
         predicate: GetPredicateForUser(),
         include: x => x.Include(u => u.UserOperationClaims).ThenInclude(u => u.OperationClaim),
         configurationProvider: mapper.ConfigurationProvider);

        return user;
    }

    public Expression<Func<User, bool>> GetPredicateForUser(Expression<Func<User, bool>>? expression = null)
    {
        var predicate = PredicateBuilder.New<User>(true);

        if (expression != null)
            predicate = predicate.And(expression);

        predicate = predicate.And(commonService.HttpUserType switch
        {
            UserTypes.Administator => x => x.Id != 1,
            UserTypes.School => x => x.Id != 1 && x.SchoolId == commonService.HttpSchoolId,
            UserTypes.Teacher => x => (x.Id != 1 && x.SchoolId == commonService.HttpSchoolId && x.Type == UserTypes.Student) || x.Id == commonService.HttpUserId,
            UserTypes.Student => x => x.Id != 1 && x.Type == UserTypes.Student,
            UserTypes.Person => x => x.Id != 1 && x.Type == UserTypes.Person,
            _ => throw new NotImplementedException(),
        });

        return predicate;
    }

    public async Task<User> PassiveUser(User user)
    {
        user.IsActive = false;
        var result = await userDal.UpdateAsyncCallback(user);
        return result;
    }

    public async Task<bool> UserStatusAndLicense(long userId)
    {
        return await cacheManager.GetOrAddAsync($"{Strings.CacheStatusAndLicence}-{userId}", async () =>
        {


            var userType = commonService.HttpUserType;
            var schoolId = commonService.HttpSchoolId;

            bool result = false;

            if (userType is UserTypes.Administator) result = true;
            else if (schoolId != null && userType is UserTypes.School or UserTypes.Teacher or UserTypes.Student)
            {
                var schoolUser = await userDal.GetAsync(
                    enableTracking: false,
                    predicate: x => x.IsActive && x.SchoolId == schoolId && x.Type==UserTypes.School && x.School != null && x.School.IsActive,
                    include: x => x.Include(u => u.PackageUsers).Include(x => x.School),
                    selector: x => new { x.PackageUsers, AccessStundents = x.School != null && x.School.AccessStundents });

                var licenseEndDate =  schoolUser.PackageUsers.Max(x => x.EndDate);

                await SchoolRules.SchoolShouldExists(schoolUser);
                await UserRules.LicenceIsValid(licenseEndDate);
                await SchoolRules.AccessStudentEnabled(schoolUser.AccessStundents, userType);

                result = await userDal.IsExistsAsync(predicate: x => x.Id == userId && x.IsActive, enableTracking: false);
            }
            else if (userType is UserTypes.Person)
            {
                var user = await userDal.GetAsync(
                    enableTracking: false,
                    predicate: x => x.Id == userId && x.IsActive,
                    selector: x => new { x.IsActive, x.PackageUsers });

                var licenseEndDate = user.PackageUsers.Max(x => x.EndDate);

                await UserRules.LicenceIsValid(licenseEndDate);
                result = true;
            }

            return result;
        }, 60);
    }
}