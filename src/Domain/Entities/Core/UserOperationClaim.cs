using Security = OCK.Core.Security.Entities;

namespace Domain.Entities.Core;

public class UserOperationClaim : Security.UserOperationClaim
{
    public virtual User User { get; set; }
    public virtual OperationClaim OperationClaim { get; set; }

    public UserOperationClaim() : base()
    {
    }

    public UserOperationClaim(int id, long userId, int operationClaimId)
        : base(id, userId, operationClaimId)
    {
    }
}