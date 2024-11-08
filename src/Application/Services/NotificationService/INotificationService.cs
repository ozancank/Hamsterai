using Application.Features.Notifications.Dto;

namespace Application.Services.NotificationService;

public interface INotificationService : IBusinessService
{
    Task<bool> PushNotificationAll(string title, string body);

    Task<bool> PushNotificationByUserId(NotificationUserDto dto);
}