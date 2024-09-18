using Domain.Entities.Core;

namespace DataAccess.Abstract;

public interface IUserOperationClaimDal : ISyncRepository<UserOperationClaim>, IAsyncRepository<UserOperationClaim>
{
}