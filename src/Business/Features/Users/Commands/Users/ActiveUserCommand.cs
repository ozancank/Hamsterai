using Business.Features.Users.Rules;
using Business.Services.UserService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Users.Commands.Users;

public class ActiveUserCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public long Id { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.School, UserTypes.Teacher];
    public string[] HidePropertyNames { get; } = [];
}

public class ActiveUserCommandHandler(IUserDal userDal,
                                      IUserService userService,
                                      UserRules userRules) : IRequestHandler<ActiveUserCommand, bool>
{
    public async Task<bool> Handle(ActiveUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userDal.GetAsync(predicate: userService.GetPredicateForUser(x => x.Id == request.Id), cancellationToken: cancellationToken);
        await UserRules.UserShouldExists(user);
        await userRules.UserTypeAllowed(user.Type, user.Id);
        user.IsActive = true;

        await userDal.UpdateAsync(user);
        return true;
    }
}