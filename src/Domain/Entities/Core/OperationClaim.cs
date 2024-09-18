using Security = OCK.Core.Security.Entities;

namespace Domain.Entities.Core;

public class OperationClaim : Security.OperationClaim
{
    public virtual ICollection<UserOperationClaim> UserOperationClaims { get; set; }

    public OperationClaim()
    {
        UserOperationClaims = [];
    }

    public OperationClaim(int id, string name, string description = null, int parentId = 0)
        : base(id, name, description, parentId)
    {
        UserOperationClaims = [];
    }
}