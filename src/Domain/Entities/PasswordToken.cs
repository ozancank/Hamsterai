using Domain.Entities.Core;
using OCK.Core.Interfaces;

namespace Domain.Entities;

public class PasswordToken : IEntity
{
    public Guid Id { get; set; }
    public DateTime CreateDate { get; set; }
    public long UserId { get; set; }
    public string Token { get; set; }
    public DateTime Expried { get; set; }

    public virtual User User { get; set; }

    public PasswordToken()
    {
    }

    public PasswordToken(Guid id, long userId, string token, DateTime expried)
    {
        Id = id;
        UserId = userId;
        Token = token;
        Expried = expried;
    }
}