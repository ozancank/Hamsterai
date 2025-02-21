using Domain.Constants;
using OCK.Core.Exceptions.CustomExceptions;
using OCK.Core.Utilities;
using Api = OneSignalApi;

namespace Infrastructure.Notification.OneSignal;

public sealed class OneSignalApi : INotificationApi
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

        List<Api.Model.Filter> filters = [];

        foreach (var token in tokens!.Cast<string>())
        {
            filters.Add(new("tag", "user_id", token, Api.Model.Filter.RelationEnum.Equal));
        }

        var appConfig = new Api.Client.Configuration
        {
            BasePath = OneSignalConfiguration.BaseUrl,
            AccessToken = OneSignalConfiguration.SecretKey,
        };
        var appInstance = new Api.Api.DefaultApi(appConfig);

        var notification = new Api.Model.Notification(appId: OneSignalConfiguration.AppId)
        {
            Headings = new Api.Model.StringMap(tr: notificationModel.Title),
            Contents = new Api.Model.StringMap(tr: notificationModel.Body),
            Data = notificationModel.Datas,
            Filters = filters
        };


        var response = await appInstance.CreateNotificationAsync(notification);

        return response.Id.IsNotEmpty();
    }
}