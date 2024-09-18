using Business.Features.Users.Rules;
using Business.Services.CommonService;
using Business.Services.UserService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;
using OCK.Core.Security.HashingHelper;

namespace Business.Features.Users.Commands.Password;

public class PasswordChangeByUserCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public string Password { get; set; }

    public UserTypes[] Roles { get; } = [];
    public string[] HidePropertyNames { get; } = ["Password"];
}

public class PasswordChangeByUserCommandHandler(IUserDal userDal,
                                                IUserService userService,
                                                ICommonService commonService,
                                                UserRules userRules) : IRequestHandler<PasswordChangeByUserCommand, bool>
{
    public async Task<bool> Handle(PasswordChangeByUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userDal.GetAsync(predicate: userService.GetPredicateForUser(x => x.Id == commonService.HttpUserId), cancellationToken: cancellationToken);

        await UserRules.UserShouldExistsAndActive(user);
        await userRules.UserTypeAllowed(user.Type, user.Id);

        HashingHelper.CreatePasswordHash(request.Password!, out byte[] passwordHash, out byte[] passwordSalt);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;
        user.MustPasswordChange = false;

        var updatedUser = await userDal.UpdateAsyncCallback(user);
        return updatedUser != null;
    }
}

public class PasswordChangeByUserCommandHandlerValidator : AbstractValidator<PasswordChangeByUserCommand>
{
    public PasswordChangeByUserCommandHandlerValidator()
    {
        RuleFor(x => x.Password)
                .NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Password])
                .MinimumLength(8).WithMessage(Strings.DynamicMinLength, [Strings.Password, "8"])
                .Matches("[a-zA-Z]").WithMessage(Strings.PasswordLetter)
                .Matches("[0-9]").WithMessage(Strings.PasswordNumber);
    }
}