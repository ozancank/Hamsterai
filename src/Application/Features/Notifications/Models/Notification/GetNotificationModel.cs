namespace Application.Features.Notifications.Models.Notification;

public class GetNotificationModel : IResponseModel
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
    public long CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public long UpdateUser { get; set; }
    public long ReceiveredUserId { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadDate { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
    public NotificationTypes Type { get; set; }
    public Guid? ReasonId { get; set; }
    public string? SenderName { get; set; }
    public string? ReceiveredName { get; set; }
}