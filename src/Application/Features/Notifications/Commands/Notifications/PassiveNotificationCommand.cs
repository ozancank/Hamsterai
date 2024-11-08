using Application.Features.Notifications.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Notifications.Commands.Notifications;

public class PassiveNotificationCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public Guid Id { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class PassiveNotificationCommandHandler(INotificationDal notificationDal,
                                               ICommonService commonService) : IRequestHandler<PassiveNotificationCommand, bool>
{
    public async Task<bool> Handle(PassiveNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await notificationDal.GetAsync(x => x.Id == request.Id && x.ReceiveredUserId == commonService.HttpUserId, cancellationToken: cancellationToken);
        await NotificationRules.NotificationShouldExists(notification);

        var date = DateTime.Now;

        notification.UpdateUser = commonService.HttpUserId;
        notification.UpdateDate = date;
        notification.IsActive = false;

        await notificationDal.UpdateAsync(notification, cancellationToken: cancellationToken);
        return true;
    }
}