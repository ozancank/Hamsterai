using OCK.Core.Interfaces;

namespace Infrastructure.Notification;

public sealed class NotificationModel<T> : IRequestModel
{
    public string? Title { get; set; }
    public string? Body { get; set; }

    public IEnumerable<T?>? List { get; set; }
}