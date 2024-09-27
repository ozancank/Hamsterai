using DataAccess.Abstract.Core;
using Domain.Entities.Core;

namespace DataAccess.EF.Concrete.Core;

public class OperationClaimDal(HamsteraiDbContext context) : EfRepositoryBase<OperationClaim, HamsteraiDbContext>(context), IOperationClaimDal
{
}