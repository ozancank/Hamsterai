using Application.Features.Notifications.Models.DeviceToken;

namespace Application.Features.Notifications.Profiles;

public class NotificationDeviceTokenMappingProfiles : Profile
{
    public NotificationDeviceTokenMappingProfiles()
    {
        CreateMap<DeviceTokenModel, NotificationDeviceToken>().ReverseMap();
    }
}