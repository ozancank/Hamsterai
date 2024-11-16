using Application.Features.Notifications.Models.Notification;

namespace Application.Features.Notifications.Profiles;

public class NotificationMappingProfiles : Profile
{
    public NotificationMappingProfiles()
    {
        CreateMap<Notification, GetNotificationModel>()
            .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.SenderUser!=null?  $"{src.SenderUser.Name} {src.SenderUser.Surname}".Trim():default))
            .ForMember(dest => dest.ReceiveredName, opt => opt.MapFrom(src => src.ReceiveredUser != null ? $"{src.ReceiveredUser.Name} {src.ReceiveredUser.Surname}".Trim() : default));
        CreateMap<IPaginate<GetNotificationModel>, PageableModel<GetNotificationModel>>();
    }
}