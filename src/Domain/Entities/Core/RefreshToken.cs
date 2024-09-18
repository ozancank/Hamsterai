using Security = OCK.Core.Security.Entities;

namespace Domain.Entities.Core;

public class RefreshToken : Security.RefreshToken
{
    public virtual User User { get; set; }

    public RefreshToken()
    {
    }
}