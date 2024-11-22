using Application.Features.Notifications.Dto;

namespace Application.Services.NotificationService;

public interface INotificationService : IBusinessService
{
    Task<bool> PushNotificationAll(string title, string body, IReadOnlyDictionary<string, string> datas);

    Task<bool> PushNotificationByUserId(NotificationUserDto dto);
}