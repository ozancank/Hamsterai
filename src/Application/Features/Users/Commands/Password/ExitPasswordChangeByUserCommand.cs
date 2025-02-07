using Application.Features.Users.Models.Password;
using Application.Features.Users.Rules;
using Application.Services.CommonService;
using Application.Services.UserService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Users.Commands.Password;

public class ExitPasswordChangeByUserCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required UpdateExitPasswordModel Model { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [$"{nameof(Model)}.{nameof(Model.OldPassword)}", $"{nameof(Model)}.{nameof(Model.Password)}"];
}

public class ExitPasswordChangeByUserCommandHandler(
    IUserDal userDal,
    IUserService userService,
    ICommonService commonService,
    UserRules userRules) : IRequestHandler<ExitPasswordChangeByUserCommand, bool>
{
    public async Task<bool> Handle(ExitPasswordChangeByUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userDal.GetAsync(predicate: userService.GetPredicateForUser(x => x.Id == commonService.HttpUserId), cancellationToken: cancellationToken);

        await UserRules.UserShouldExistsAndActive(user);
        await UserRules.ExitPasswordShouldVerifiedWhenPasswordChange(user.ExitPassword!, request.Model.OldPassword!);
        await userRules.UserTypeAllowed(user.Type, user.Id, true);

        var encryptedText = CryptographyTools.EncryptWithAes256(request.Model.Password?.Trim(), AppOptions.ExitPassKeyword, AppOptions.ExitPassVector);

        user.ExitPassword = encryptedText;

        var updatedUser = await userDal.UpdateAsyncCallback(user, cancellationToken: cancellationToken);
        return updatedUser != null;
    }
}

public class ExitPasswordChangeByUserCommandValidator : AbstractValidator<ExitPasswordChangeByUserCommand>
{
    public ExitPasswordChangeByUserCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.OldPassword).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.OldPassword]);

        RuleFor(x => x.Model.Password).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Password]);
    }
}