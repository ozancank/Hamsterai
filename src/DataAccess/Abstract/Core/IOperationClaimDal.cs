using Domain.Entities.Core;

namespace DataAccess.Abstract.Core;

public interface IOperationClaimDal : ISyncRepository<OperationClaim>, IAsyncRepository<OperationClaim>
{
}