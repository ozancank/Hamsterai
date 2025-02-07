namespace Application.Features.Notifications.Rules;

public class NotificationRules(INotificationDal notificationDal) : IBusinessRule
{
    internal static Task NotificationShouldExists(object model)
    {
        if (model == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Notification);
        return Task.CompletedTask;
    }

    internal async Task NotificationShouldExistsById(Guid notificationId)
    {
        var entity = await notificationDal.IsExistsAsync(predicate: x => x.Id == notificationId, enableTracking: false);
        if (!entity) throw new BusinessException(Strings.DynamicNotFound, Strings.Notification);
    }

    internal async Task NotificationShouldExistsAndActiveById(Guid notificationId)
    {
        var entity = await notificationDal.IsExistsAsync(predicate: x => x.Id == notificationId && x.IsActive, enableTracking: false);
        if (!entity) throw new BusinessException(Strings.DynamicNotFound, Strings.Notification);
    }
}