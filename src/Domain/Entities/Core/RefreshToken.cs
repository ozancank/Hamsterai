using Security = OCK.Core.Security.Entities;

namespace Domain.Entities.Core;

public class RefreshToken : Security.RefreshToken
{
    public virtual User? User { get; set; }

    public RefreshToken():base()
    {    }

    public RefreshToken(long id, long userId, string token, DateTime expires, DateTime created, string createdByIp, DateTime? revoked,
                    string revokedByIp, string replacedByToken, string reasonRevoked) : this()
    {
        Id = id;
        UserId = userId;
        Token = token;
        Expires = expires;
        Created = created;
        CreatedByIp = createdByIp;
        Revoked = revoked;
        RevokedByIp = revokedByIp;
        ReplacedByToken = replacedByToken;
        ReasonRevoked = reasonRevoked;
    }
}