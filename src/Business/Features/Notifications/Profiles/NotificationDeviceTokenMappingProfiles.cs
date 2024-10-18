using Business.Features.Notifications.Models.DeviceToken;

namespace Business.Features.Notifications.Profiles;

public class NotificationDeviceTokenMappingProfiles : Profile
{
    public NotificationDeviceTokenMappingProfiles()
    {
        CreateMap<DeviceTokenModel, NotificationDeviceToken>().ReverseMap();
    }
}