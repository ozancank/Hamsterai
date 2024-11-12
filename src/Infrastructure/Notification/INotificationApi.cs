using OCK.Core.Interfaces;

namespace Infrastructure.Notification;

public interface INotificationApi : IExternalApi
{
    Task<bool> PushNotification<T>(NotificationModel<T> notificationModel);
}