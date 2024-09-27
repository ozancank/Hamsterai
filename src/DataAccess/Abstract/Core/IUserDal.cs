using Domain.Entities.Core;

namespace DataAccess.Abstract.Core;

public interface IUserDal : ISyncRepository<User>, IAsyncRepository<User>
{
}