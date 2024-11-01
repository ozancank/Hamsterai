using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Notifications.Commands.DeviceToken;

public class PassiveDeviceTokenCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public long UserId { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class PassiveDeviceTokenDeviceTokenHandler(INotificationDeviceTokenDal deviceTokenDal) : IRequestHandler<PassiveDeviceTokenCommand, bool>
{
    public async Task<bool> Handle(PassiveDeviceTokenCommand request, CancellationToken cancellationToken)
    {
        var tokens = await deviceTokenDal.GetListAsync(predicate: x => x.UserId == request.UserId, cancellationToken: cancellationToken);

        foreach (var token in tokens)
        {
            token.IsActive = false;
        }

        await deviceTokenDal.UpdateRangeAsync(tokens, cancellationToken: cancellationToken);
        return true;
    }
}