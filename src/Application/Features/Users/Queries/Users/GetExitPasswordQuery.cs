using Application.Features.Users.Models.User;
using Application.Features.Users.Rules;
using Application.Services.UserService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Users.Queries.Users;

public class GetExitPasswordQuery : IRequest<string>, ISecuredRequest<UserTypes>
{
    public required long UserId { get; set; }
    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetExitPasswordQueryHandler(IUserDal userDal,
                                         IUserService userService) : IRequestHandler<GetExitPasswordQuery, string>
{
    public async Task<string> Handle(GetExitPasswordQuery request, CancellationToken cancellationToken)
    {
        var user = await userDal.GetAsync(
            enableTracking: false,
            predicate: userService.GetPredicateForUser(x => x.Id == request.UserId),
            selector: x => x.ExitPassword,
            cancellationToken: cancellationToken);

        await UserRules.UserShouldExists(user);
        
        return user!;
    }
}