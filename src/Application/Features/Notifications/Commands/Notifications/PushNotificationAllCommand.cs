﻿using Application.Features.Notifications.Models.Notification;
using Application.Services.NotificationService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Notifications.Commands.Notifications;

public class PushNotificationAllCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required MessageRequestModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class PushNotificationAllCommandHandler(INotificationService notificationService) : IRequestHandler<PushNotificationAllCommand, bool>
{
    public async Task<bool> Handle(PushNotificationAllCommand request, CancellationToken cancellationToken)
    {
        request.Model.Datas ??= [];
        var result = await notificationService.PushNotificationAll(request.Model.Title!, request.Model.Body!, request.Model.Datas);
        return result;
    }
}

public class PushNotificationAllCommandValidator : AbstractValidator<MessageRequestModel>
{
    public PushNotificationAllCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Title).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Header]);

        RuleFor(x => x.Body).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Message]);
    }
}