using Domain.Constants;
using Domain.Entities.Core;

namespace Domain.Entities;

public class Notification : BaseEntity<Guid>
{
    public long ReceiveredUserId { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadDate { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
    public NotificationTypes Type { get; set; }
    public string? ReasonId { get; set; }

    public virtual User? SenderUser { get; set; }
    public virtual User? ReceiveredUser { get; set; }

    public Notification() : base()
    { }

    public Notification(Guid id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    { }
}