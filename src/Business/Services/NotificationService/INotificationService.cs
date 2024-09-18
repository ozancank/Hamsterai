namespace Business.Services.NotificationService;

public interface INotificationService : IBusinessService
{
    Task<bool> PushNotificationAll(string title, string body);

    Task<bool> PushNotificationByUserId(string title, string body, long userId);
}