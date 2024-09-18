using Domain.Entities.Core;

namespace DataAccess.EF.Concrete;

public class UserOperationClaimDal(HamsteraiDbContext context) : EfRepositoryBase<UserOperationClaim, HamsteraiDbContext>(context), IUserOperationClaimDal
{
}