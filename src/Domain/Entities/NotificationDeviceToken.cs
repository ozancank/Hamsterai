using Domain.Entities.Core;
using OCK.Core.Interfaces;

namespace Domain.Entities;

public class NotificationDeviceToken : IEntity
{
    public Guid Id { get; set; }
    public DateTime CreateDate { get; set; }
    public bool IsActive { get; set; }
    public long UserId { get; set; }
    public string? DeviceToken { get; set; }

    public virtual User? User { get; set; }

    public NotificationDeviceToken()
    { }

    public NotificationDeviceToken(Guid id, bool isActive, long createUser, DateTime createDate) : this()
    {
        Id = id;
        IsActive = isActive;
        CreateDate = createDate;
    }
}