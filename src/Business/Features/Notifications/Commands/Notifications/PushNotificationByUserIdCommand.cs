using Business.Features.Notifications.Models.Notification;
using Business.Services.NotificationService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Notifications.Commands.Notifications;

public class PushNotificationByUserIdCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required MessageRequestModel Model { get; set; }
    public long UserId { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class PushNotificationByUserIdCommandHandler(INotificationService notificationService)
    : IRequestHandler<PushNotificationByUserIdCommand, bool>
{
    public async Task<bool> Handle(PushNotificationByUserIdCommand request, CancellationToken cancellationToken)
    {
        var result = await notificationService.PushNotificationByUserId(new(request.Model.Title!, request.Model.Body!, request.UserId, NotificationTypes.Undifined));
        return result;
    }
}

public class PushNotificationByUserIdCommandValidator : AbstractValidator<PushNotificationByUserIdCommand>
{
    public PushNotificationByUserIdCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotNull().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.Title).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Header]);

        RuleFor(x => x.Model.Body).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Message]);
    }
}