using Security = OCK.Core.Security.Entities;

namespace Domain.Entities.Core;

public class OperationClaim : Security.OperationClaim
{
    public virtual ICollection<UserOperationClaim> UserOperationClaims { get; set; } = [];

    public OperationClaim() : base()
    { }

    public OperationClaim(int id, string name, string description = "", int parentId = 0)
        : base(id, name, description, parentId)
    { }
}