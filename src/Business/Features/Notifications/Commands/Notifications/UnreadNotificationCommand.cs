using Business.Features.Notifications.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Notifications.Commands.Notifications;

public class UnreadNotificationCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public Guid Id { get; set; }

    public UserTypes[] Roles { get; } = [];
    public string[] HidePropertyNames { get; } = [];
}

public class UnReadNotificationCommandHandler(INotificationDal notificationDal,
                                              ICommonService commonService) : IRequestHandler<UnreadNotificationCommand, bool>
{
    public async Task<bool> Handle(UnreadNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await notificationDal.GetAsync(x => x.Id == request.Id && x.ReceiveredUserId == commonService.HttpUserId, cancellationToken: cancellationToken);
        await NotificationRules.NotificationShouldExists(notification);

        var date = DateTime.Now;

        notification.UpdateUser = commonService.HttpUserId;
        notification.UpdateDate = date;
        notification.IsRead = false;

        await notificationDal.UpdateAsync(notification, cancellationToken: cancellationToken);
        return true;
    }
}