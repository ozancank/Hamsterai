using Domain.Entities.Core;

namespace DataAccess.EF.Concrete;

public class RefreshTokenDal(HamsteraiDbContext context) : EfRepositoryBase<RefreshToken, HamsteraiDbContext>(context), IRefreshTokenDal
{
}