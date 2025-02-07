using Domain.Entities.Core;
using System.Linq.Expressions;

namespace Application.Services.UserService;

public interface IUserService : IBusinessService
{
    Task<User> GetUserById(long id, bool tracking = false);

    Task<T> GetUserById<T>(long id, bool tracking = false) where T : class, new();

    Expression<Func<User, bool>> GetPredicateForUser(Expression<Func<User, bool>>? expression = null);

    Task<User> PassiveUser(User user);

    Task<bool> UserStatusAndLicense(long id);

    Task<int> RemainigQuestionCredit(long userId);

    Task<int> ValidTotalQuestion(long userId);
}