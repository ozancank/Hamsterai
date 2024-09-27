using DataAccess.Abstract.Core;
using Domain.Entities.Core;

namespace DataAccess.EF.Concrete.Core;

public class UserOperationClaimDal(HamsteraiDbContext context) : EfRepositoryBase<UserOperationClaim, HamsteraiDbContext>(context), IUserOperationClaimDal
{
}