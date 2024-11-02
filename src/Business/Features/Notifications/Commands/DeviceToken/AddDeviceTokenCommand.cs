using Business.Features.Notifications.Models.DeviceToken;
using Business.Features.Packages.Models.Packages;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Notifications.Commands.DeviceToken;

public class AddDeviceTokenCommand : IRequest<DeviceTokenModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required DeviceTokenModel Model { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class AddDeviceTokenpCommandHandler(IMapper mapper,
                                           INotificationDeviceTokenDal deviceTokenDal,
                                           ICommonService commonService) : IRequestHandler<AddDeviceTokenCommand, DeviceTokenModel>
{
    public async Task<DeviceTokenModel> Handle(AddDeviceTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await deviceTokenDal.GetAsync(
            predicate: x => x.UserId == commonService.HttpUserId,
            cancellationToken: cancellationToken);

        DeviceTokenModel result;
        if (token != null)
        {
            token.DeviceToken = request.Model.DeviceToken;
            var updated = await deviceTokenDal.UpdateAsyncCallback(token, cancellationToken: cancellationToken);
            result = mapper.Map<DeviceTokenModel>(updated);
        }
        else
        {
            var entity = mapper.Map<NotificationDeviceToken>(request.Model);
            entity.Id = Guid.NewGuid();
            entity.UserId = commonService.HttpUserId;
            entity.CreateDate = DateTime.Now;
            entity.IsActive = true;
            var added = await deviceTokenDal.AddAsyncCallback(entity, cancellationToken: cancellationToken);
            result = mapper.Map<DeviceTokenModel>(added);
        }

        return result;
    }
}

public class AddDeviceTokenpCommandValidator : AbstractValidator<GetPackageModel>
{
    public AddDeviceTokenpCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Name).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Name).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Name).MaximumLength(50).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "50"]);
    }
}