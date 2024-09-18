using Business.Features.Users.Rules;
using Business.Services.UserService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;
using OCK.Core.Security.HashingHelper;

namespace Business.Features.Users.Commands.Password;

public class PasswordChangeByManagerCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public long Id { get; set; }
    public string Password { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.School];
    public string[] HidePropertyNames { get; } = ["Password"];
}

public class PasswordChangeByManagerCommandHandler(IUserDal userDal,
                                                   IUserService userService,
                                                   UserRules userBusinessRules) : IRequestHandler<PasswordChangeByManagerCommand, bool>
{
    public async Task<bool> Handle(PasswordChangeByManagerCommand request, CancellationToken cancellationToken)
    {
        var user = await userDal.GetAsync(predicate: userService.GetPredicateForUser(x => x.Id == request.Id), cancellationToken: cancellationToken);

        await UserRules.UserShouldExistsAndActive(user);
        await userBusinessRules.UserTypeAllowed(user.Type, user.Id);

        HashingHelper.CreatePasswordHash(request.Password!, out byte[] passwordHash, out byte[] passwordSalt);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;
        user.MustPasswordChange = false;

        var updatedUser = await userDal.UpdateAsyncCallback(user);
        return updatedUser != null;
    }
}

public class ManagerPasswordChangeCommandHandlerValidator : AbstractValidator<PasswordChangeByManagerCommand>
{
    public ManagerPasswordChangeCommandHandlerValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(Strings.IdNotEmpty);

        RuleFor(x => x.Password)
                .NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Password])
                .MinimumLength(8).WithMessage(Strings.DynamicMinLength, [Strings.Password, "8"])
                .Matches("[a-zA-Z]").WithMessage(Strings.PasswordLetter)
                .Matches("[0-9]").WithMessage(Strings.PasswordNumber);
    }
}