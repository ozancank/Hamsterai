using Business.Features.Users.Models.User;
using Business.Features.Users.Rules;
using Business.Services.UserService;
using MediatR;

namespace Business.Features.Users.Queries.Users;

public class GetUserByIdQuery : IRequest<GetUserModel>
{
    public long Id { get; set; }
}

public class GetUserByIdQueryHandler(IMapper mapper,
                                     IUserDal userDal,
                                     IUserService userService) : IRequestHandler<GetUserByIdQuery, GetUserModel>
{
    public async Task<GetUserModel> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userDal.GetAsyncAutoMapper<GetUserModel>(
            enableTracking: false,
            predicate: userService.GetPredicateForUser(x => x.Id == request.Id),
            include: x => x.Include(u => u.UserOperationClaims).ThenInclude(u => u.OperationClaim),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        await UserRules.UserShouldExistsAndActive(user);
        return user;
    }
}