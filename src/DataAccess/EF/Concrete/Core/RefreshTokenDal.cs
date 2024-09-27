using DataAccess.Abstract.Core;
using Domain.Entities.Core;

namespace DataAccess.EF.Concrete.Core;

public class RefreshTokenDal(HamsteraiDbContext context) : EfRepositoryBase<RefreshToken, HamsteraiDbContext>(context), IRefreshTokenDal
{
}