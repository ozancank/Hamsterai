using Domain.Entities.Core;
using System.Linq.Expressions;

namespace Business.Services.UserService;

public interface IUserService : IBusinessService
{
    Task<User> GetUserById(long id, bool tracking = false);

    Task<T> GetUserById<T>(long id, bool tracking = false) where T : class, new();

    Expression<Func<User, bool>> GetPredicateForUser(Expression<Func<User, bool>> expression = null);

    Task<User> PassiveUser(User user);

    Task<bool> UserStatus(long id);
}