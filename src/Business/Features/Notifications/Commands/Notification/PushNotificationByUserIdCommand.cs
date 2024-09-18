using Business.Features.Notifications.Models.Notification;
using Business.Services.NotificationService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Notifications.Commands.Notification;

public class PushNotificationByUserIdCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public MessageRequestModel Model { get; set; }
    public long UserId { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public string[] HidePropertyNames { get; } = [];
}

public class PushNotificationByUserIdCommandHandler(INotificationService notificationService)
    : IRequestHandler<PushNotificationByUserIdCommand, bool>
{
    public async Task<bool> Handle(PushNotificationByUserIdCommand request, CancellationToken cancellationToken)
    {
        var result = await notificationService.PushNotificationByUserId(request.Model.Title, request.Model.Body, request.UserId);
        return result;
    }
}

public class PushNotificationByUserIdCommandValidator : AbstractValidator<MessageRequestModel>
{
    public PushNotificationByUserIdCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Title).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Header]);

        RuleFor(x => x.Body).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Message]);
    }
}