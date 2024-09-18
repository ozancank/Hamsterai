using Domain.Entities.Core;

namespace DataAccess.EF.Concrete;

public class OperationClaimDal(HamsteraiDbContext context) : EfRepositoryBase<OperationClaim, HamsteraiDbContext>(context), IOperationClaimDal
{
}