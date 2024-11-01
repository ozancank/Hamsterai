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

    public NotificationDeviceToken(Guid id, DateTime createDate, bool isActive, long userId, string deviceToken) : this()
    {
        Id = id;
        CreateDate = createDate;
        IsActive = isActive;
        UserId = userId;
        DeviceToken = deviceToken;
    }
}