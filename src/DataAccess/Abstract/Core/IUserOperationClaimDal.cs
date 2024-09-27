using Domain.Entities.Core;

namespace DataAccess.Abstract.Core;

public interface IUserOperationClaimDal : ISyncRepository<UserOperationClaim>, IAsyncRepository<UserOperationClaim>
{
}