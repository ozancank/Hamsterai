using Business.Features.Schools.Rules;
using Business.Features.Users.Rules;
using Business.Services.CommonService;
using DataAccess.Abstract.Core;
using Domain.Entities.Core;
using LinqKit;
using OCK.Core.Caching;
using System.Linq.Expressions;

namespace Business.Services.UserService;

public class UserManager(IUserDal userDal,
                         IMapper mapper,
                         ISchoolDal schoolDal,
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

    public async Task<bool> UserStatusAndLicense(long id)
    {
        return await cacheManager.GetOrAddAsync($"{Strings.CacheStatusAndLicence}-{id}", async () =>
        {
            var userType = commonService.HttpUserType;
            var schoolId = commonService.HttpSchoolId;

            bool result = false;

            if (schoolId != null && userType is UserTypes.School or UserTypes.Teacher or UserTypes.Student)
            {
                var school = await schoolDal.GetAsync(
                    enableTracking: false,
                    predicate: x => x.Id == schoolId,
                    selector: x => new { x.LicenseEndDate, x.IsActive });

                await UserRules.LicenceIsValid(school.LicenseEndDate);
                await SchoolRules.SchoolShouldExists(school.IsActive);
                result = await userDal.IsExistsAsync(predicate: x => x.Id == id && x.IsActive, enableTracking: false);
            }
            else if (userType is UserTypes.Person)
            {
                var user = await userDal.GetAsync(
                    enableTracking: false,
                    predicate: x => x.Id == id && x.IsActive,
                    selector: x => new { x.IsActive, x.LicenceEndDate });
                await UserRules.LicenceIsValid(user.LicenceEndDate);
                result = true;
            }

            return result;
        }, 60);
    }
}