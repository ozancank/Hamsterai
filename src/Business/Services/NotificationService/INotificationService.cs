using Business.Features.Notifications.Dto;

namespace Business.Services.NotificationService;

public interface INotificationService : IBusinessService
{
    Task<bool> PushNotificationAll(string title, string body);

    Task<bool> PushNotificationByUserId(NotificationUserDto dto);
}