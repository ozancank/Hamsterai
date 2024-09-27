using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Notifications.Commands.DeviceToken;

public class ActiveDeviceTokenCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public long UserId { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public string[] HidePropertyNames { get; } = [];
}

public class ActiveDeviceTokenHandler(INotificationDeviceTokenDal deviceTokenDal) : IRequestHandler<ActiveDeviceTokenCommand, bool>
{
    public async Task<bool> Handle(ActiveDeviceTokenCommand request, CancellationToken cancellationToken)
    {
        var tokens = await deviceTokenDal.GetListAsync(predicate: x => x.UserId == request.UserId, cancellationToken: cancellationToken);

        foreach (var token in tokens)
        {
            token.IsActive = true;
        }

        await deviceTokenDal.UpdateRangeAsync(tokens, cancellationToken: cancellationToken);
        return true;
    }
}