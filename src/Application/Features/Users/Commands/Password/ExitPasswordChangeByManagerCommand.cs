using Application.Features.Users.Models.Password;
using Application.Features.Users.Rules;
using Application.Services.UserService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Users.Commands.Password;

public class ExitPasswordChangeByManagerCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required UpdateExitPasswordModel Model { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [$"{nameof(Model)}.{nameof(Model.OldPassword)}", $"{nameof(Model)}.{nameof(Model.Password)}"];
}

public class ExitPasswordChangeByManagerCommandHandler(
    IUserDal userDal,
    IUserService userService,
    UserRules userRules) : IRequestHandler<ExitPasswordChangeByManagerCommand, bool>
{
    public async Task<bool> Handle(ExitPasswordChangeByManagerCommand request, CancellationToken cancellationToken)
    {
        await userRules.UserCanNotChangeOwnOrAdminPassword(request.Model.Id);

        var user = await userDal.GetAsync(predicate: userService.GetPredicateForUser(x => x.Id == request.Model.Id), cancellationToken: cancellationToken);

        await UserRules.UserShouldExistsAndActive(user);
        await userRules.UserTypeAllowed(user.Type, user.Id);

        var encryptedText = CryptographyTools.EncryptWithAes256(request.Model.Password?.Trim(), AppOptions.ExitPassKeyword, AppOptions.ExitPassVector);

        user.ExitPassword = encryptedText;

        var updatedUser = await userDal.UpdateAsyncCallback(user, cancellationToken: cancellationToken);
        return updatedUser != null;
    }
}

public class ExitPasswordChangeByManagerCommandValidator : AbstractValidator<ExitPasswordChangeByManagerCommand>
{
    public ExitPasswordChangeByManagerCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.Id).NotEmpty().WithMessage(Strings.IdNotEmpty);

        RuleFor(x => x.Model.Password).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Password]);
    }
}