using Infrastructure.Notification;

namespace Business.Services.NotificationService;

public class NotificationManager(INotificationApi notificationApi,
                                 INotificationDeviceTokenDal notificationDeviceTokenDal) : INotificationService
{
    public async Task<bool> PushNotificationAll(string title, string body)
    {
        var tokens = await notificationDeviceTokenDal.Query().AsNoTracking().Include(x => x.User)
            .Where(x => x.IsActive && x.User.IsActive)
            .Select(x => x.DeviceToken).ToListAsync();

        if (tokens.Count == 0) return false;

        var message = new NotificationModel<string>()
        {
            Title = title,
            Body = body,

            ToList = tokens
        };

        _ = notificationApi.PushNotification(message);
        return true;
    }

    public async Task<bool> PushNotificationByUserId(string title, string body, long userId)
    {
        var tokens = await notificationDeviceTokenDal.Query().AsNoTracking().Include(x => x.User)
            .Where(x => x.UserId == userId && x.IsActive && x.User.IsActive)
            .Select(x => x.DeviceToken).ToListAsync();

        if (tokens.Count == 0) return false;

        var message = new NotificationModel<string>()
        {
            Title = title,
            Body = body,

            ToList = tokens
        };

        _ = notificationApi.PushNotification(message);
        return true;
    }
}