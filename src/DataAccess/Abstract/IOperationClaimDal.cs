using Domain.Entities.Core;

namespace DataAccess.Abstract;

public interface IOperationClaimDal : ISyncRepository<OperationClaim>, IAsyncRepository<OperationClaim>
{
}