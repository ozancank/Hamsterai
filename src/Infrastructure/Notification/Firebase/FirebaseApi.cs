using Domain.Constants;
using FirebaseAdmin.Messaging;
using OCK.Core.Exceptions.CustomExceptions;
using OCK.Core.Utilities;

namespace Infrastructure.Notification.Firebase;

public sealed class FirebaseApi : INotificationApi
{
    private static bool? _isNullable = null;
    private static bool? _isString = null;

    public async Task<bool> PushNotification<T>(NotificationModel<T> notificationModel)
    {
        ArgumentNullException.ThrowIfNull(notificationModel);

        _isNullable ??= ReflectionTools.IsNullableType(typeof(T));
        _isString ??= typeof(T) == typeof(string);

        if (!_isString.Value) throw new BusinessException(Strings.InvalidValue);

        if (_isNullable.Value && notificationModel.List == null)
            throw new BusinessException(Strings.InvalidValue);

        var tokens = notificationModel.List?.ToList();

        if (tokens?.Count == 0) throw new BusinessException(Strings.DynamicNotEmpty, Strings.Token);

        var message = new MulticastMessage()
        {
            Notification = new FirebaseAdmin.Messaging.Notification
            {
                Title = notificationModel.Title,
                Body = notificationModel.Body,
            },
            Data = notificationModel.Datas,
            Tokens = tokens as List<string>,
        };

        var messaging = FirebaseMessaging.DefaultInstance;
        _ = await messaging.SendEachForMulticastAsync(message);

        return true;
    }
}