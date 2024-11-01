using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Notifications.Commands.Notifications;

public class ReadAllNotificationCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class ReadAllNotificationCommandHandler(INotificationDal notificationDal,
                                               ICommonService commonService) : IRequestHandler<ReadAllNotificationCommand, bool>
{
    public async Task<bool> Handle(ReadAllNotificationCommand request, CancellationToken cancellationToken)
    {
        var notifications = await notificationDal.GetListAsync(predicate: x => x.ReceiveredUserId == commonService.HttpUserId && !x.IsRead, cancellationToken: cancellationToken);

        var date = DateTime.Now;

        foreach (var notification in notifications)
        {
            notification.UpdateUser = commonService.HttpUserId;
            notification.UpdateDate = date;
            notification.IsRead = true;
            notification.ReadDate = date;
        }

        await notificationDal.UpdateRangeAsync(notifications, cancellationToken: cancellationToken);
        return true;
    }
}