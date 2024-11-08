using Application.Features.Users.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Notifications.Commands.Notifications;

public class PassiveAllNotificationCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class PassiveAllNotificationCommandHandler(INotificationDal notificationDal,
                                                  ICommonService commonService) : IRequestHandler<PassiveAllNotificationCommand, bool>
{
    public async Task<bool> Handle(PassiveAllNotificationCommand request, CancellationToken cancellationToken)
    {
        var notifications = await notificationDal.GetListAsync(predicate: x => x.ReceiveredUserId == commonService.HttpUserId, cancellationToken: cancellationToken);

        var date = DateTime.Now;

        foreach (var notification in notifications)
        {
            notification.UpdateUser = commonService.HttpUserId;
            notification.UpdateDate = date;
            notification.IsActive = false;
        }

        await notificationDal.UpdateRangeAsync(notifications, cancellationToken: cancellationToken);
        return true;
    }
}