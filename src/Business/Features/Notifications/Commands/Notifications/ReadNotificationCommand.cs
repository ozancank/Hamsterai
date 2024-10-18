using Business.Features.Notifications.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Notifications.Commands.Notifications;

public class ReadNotificationCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public Guid Id { get; set; }

    public UserTypes[] Roles { get; } = [];
    public string[] HidePropertyNames { get; } = [];
}

public class ReadNotificationCommandHandler(INotificationDal notificationDal,
                                            ICommonService commonService) : IRequestHandler<ReadNotificationCommand, bool>
{
    public async Task<bool> Handle(ReadNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await notificationDal.GetAsync(x => x.Id == request.Id && x.ReceiveredUserId == commonService.HttpUserId, cancellationToken: cancellationToken);
        await NotificationRules.NotificationShouldExists(notification);

        var date = DateTime.Now;

        notification.UpdateUser = commonService.HttpUserId;
        notification.UpdateDate = date;
        notification.IsRead = true;
        notification.ReadDate = date;

        await notificationDal.UpdateAsync(notification, cancellationToken: cancellationToken);
        return true;
    }
}