using Domain.Entities.Core;

namespace DataAccess.Abstract.Core;

public interface IRefreshTokenDal : ISyncRepository<RefreshToken>, IAsyncRepository<RefreshToken>
{
}