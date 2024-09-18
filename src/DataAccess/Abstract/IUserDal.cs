using Domain.Entities.Core;

namespace DataAccess.Abstract;

public interface IUserDal : ISyncRepository<User>, IAsyncRepository<User>
{
}