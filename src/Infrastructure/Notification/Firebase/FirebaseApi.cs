using Domain.Constants;
using FirebaseAdmin.Messaging;
using OCK.Core.Exceptions.CustomExceptions;
using OCK.Core.Utilities;

namespace Infrastructure.Notification.Firebase;

public class FirebaseApi : INotificationApi
{
    private static bool? IsNullable = null;
    private static bool? IsString = null;

    public async Task<bool> PushNotification<T>(NotificationModel<T> notificationModel)
    {
        ArgumentNullException.ThrowIfNull(notificationModel);

        IsNullable ??= ReflectionTools.IsNullableType(typeof(T));
        IsString ??= typeof(T) == typeof(string);

        if (!IsString.Value) throw new BusinessException(Strings.InvalidValue);

        if (IsNullable.Value && notificationModel.List == null)
            throw new BusinessException(Strings.InvalidValue);

        var tokens = notificationModel.List.ToList();

        if (tokens.Count == 0) throw new BusinessException(Strings.DynamicNotEmpty, Strings.Token);

        var message = new MulticastMessage()
        {
            Notification = new FirebaseAdmin.Messaging.Notification
            {
                Title = notificationModel.Title,
                Body = notificationModel.Body,
            },
            Tokens = tokens as List<string>,
        };        

        var messaging = FirebaseMessaging.DefaultInstance;
        _ = await messaging.SendEachForMulticastAsync(message);

        return true;
    }
}