using Business.Features.Notifications.Models.DeviceToken;

namespace Business.Features.Notifications.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<DeviceTokenModel, NotificationDeviceToken>().ReverseMap();
    }
}