using Application.Features.Schools.Rules;
using Application.Features.Users.Rules;
using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using Domain.Entities.Core;
using LinqKit;
using OCK.Core.Caching;
using System.Linq.Expressions;

namespace Application.Services.UserService;

public class UserManager(IUserDal userDal,
                         IMapper mapper,
                         ICacheManager cacheManager,
                         IQuestionDal questionDal,
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
                    predicate: x => x.IsActive && x.SchoolId == schoolId && x.Type == UserTypes.School && x.School != null && x.School.IsActive,
                    include: x => x.Include(u => u.PackageUsers).Include(x => x.School),
                    selector: x => new { x.IsActive, x.PackageUsers, AccessStundents = x.School != null && x.School.AccessStundents });

                var licenseEndDate = schoolUser.PackageUsers.DefaultIfEmpty().Max(x => x != null ? x.EndDate : AppStatics.MilleniumDate);

                await SchoolRules.SchoolShouldExistsAndIsActive(schoolUser.IsActive);
                await SchoolRules.AccessStudentEnabled(schoolUser.AccessStundents, userType);
                await UserRules.LicenceIsValid(licenseEndDate);

                result = await userDal.IsExistsAsync(predicate: x => x.Id == userId && x.IsActive, enableTracking: false);
            }
            else if (userType is UserTypes.Person)
            {
                var user = await userDal.GetAsync(
                    enableTracking: false,
                    predicate: x => x.Id == userId && x.IsActive,
                    selector: x => new { x.IsActive, x.PackageUsers });
                await UserRules.UserShouldExistsAndActive(user.IsActive);

                var licenseEndDate = user.PackageUsers.DefaultIfEmpty().Max(x => x != null ? x.EndDate : AppStatics.MilleniumDate);

                await UserRules.LicenceIsValid(licenseEndDate);
                result = true;
            }

            return result;
        }, 60);
    }

    public async Task<int> RemainigQuestionCredit(long userId)
    {
        var user = await userDal.GetAsync(
            enableTracking: false,
            predicate: x => x.Id == userId && x.IsActive,
            include: x => x.Include(u => u.PackageUsers),
            selector: x => new { x.Type, x.PackageUsers, x.SchoolId });
        await UserRules.UserShouldExists(user);

        var totalCredit = 0;

        switch (user.Type)
        {
            case UserTypes.Administator:
                totalCredit = int.MaxValue;
                break;

            case UserTypes.School:
            case UserTypes.Teacher:
            case UserTypes.Student:
                var newUserId = await userDal.GetAsync(
                    enableTracking: false,
                    predicate: x => x.SchoolId == user.SchoolId && x.Type == UserTypes.School && x.IsActive,
                    selector: x => x.Id);

                var schoolUser = await userDal.GetAsync(
                    enableTracking: false,
                    predicate: x => x.Id == newUserId && x.IsActive,
                    include: x => x.Include(u => u.PackageUsers),
                    selector: x => new { x.PackageUsers });

                totalCredit = schoolUser.PackageUsers.Where(x => x.EndDate > DateTime.Now).DefaultIfEmpty().Sum(x => x != null ? x.QuestionCredit : 0);
                break;

            case UserTypes.Person:
                totalCredit = user.PackageUsers.Where(x => x.EndDate > DateTime.Now).DefaultIfEmpty().Sum(x => x != null ? x.QuestionCredit : 0);
                break;

            default:
                totalCredit = 0;
                break;
        }

        var questionCount = await questionDal.CountOfRecordAsync(
            enableTracking: false,
            predicate: x => x.CreateUser == userId && AppStatics.QuestionStatusesForCredit.Contains(x.Status) || x.ManuelSendAgain);
        return totalCredit - questionCount;
    }

    public async Task<int> ValidTotalQuestion(long userId)
    {
        var userType = commonService.HttpUserType;
        var questionCount = 0;
        if (userType == UserTypes.Administator)
            questionCount = await questionDal.CountOfRecordAsync(
                enableTracking: false,
                predicate: x => x.CreateUser == userId && AppStatics.QuestionStatusesForCredit.Contains(x.Status));
        else
            questionCount = await questionDal.CountOfRecordAsync(
                enableTracking: false,
                predicate: x => x.CreateUser == commonService.HttpUserId && AppStatics.QuestionStatusesForCredit.Contains(x.Status));
        return questionCount;
    }
}