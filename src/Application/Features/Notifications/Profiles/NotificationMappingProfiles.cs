using Application.Features.Notifications.Models.Notification;

namespace Application.Features.Notifications.Profiles;

public class NotificationMappingProfiles : Profile
{
    public NotificationMappingProfiles()
    {
        CreateMap<Notification, GetNotificationModel>()
            .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => $"{src.SenderUser!.Name} {src.SenderUser.Surname}".Trim()))
            .ForMember(dest => dest.ReceiveredName, opt => opt.MapFrom(src => $"{src.ReceiveredUser!.Name} {src.ReceiveredUser.Surname}".Trim()));
        CreateMap<IPaginate<GetNotificationModel>, PageableModel<GetNotificationModel>>();
    }
}