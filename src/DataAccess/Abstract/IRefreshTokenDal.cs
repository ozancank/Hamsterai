using Domain.Entities.Core;

namespace DataAccess.Abstract;

public interface IRefreshTokenDal : ISyncRepository<RefreshToken>, IAsyncRepository<RefreshToken>
{
}