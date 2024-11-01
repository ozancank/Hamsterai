using Business.Features.Users.Models.User;
using Business.Services.UserService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Users.Queries.Users;

public class GetUsersByDynamicQuery : IRequest<PageableModel<GetUserModel>>, ISecuredRequest<UserTypes>
{
    public required Dynamic Dynamic { get; set; }
    public required PageRequest PageRequest { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.School, UserTypes.Teacher];
    public bool AllowByPass => false;
}

public class GetUsersByDynamicHandler(IMapper mapper,
                                      IUserDal userDal,
                                      IUserService userService) : IRequestHandler<GetUsersByDynamicQuery, PageableModel<GetUserModel>>
{
    public async Task<PageableModel<GetUserModel>> Handle(GetUsersByDynamicQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.Dynamic ??= new Dynamic();

        var users = await userDal.GetPageListAsyncAutoMapperByDynamic<GetUserModel>(
            dynamic: request.Dynamic,
            predicate: userService.GetPredicateForUser(),
            defaultOrderColumnName: x => x.Name,
            enableTracking: false,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken
            );

        var result = mapper.Map<PageableModel<GetUserModel>>(users);
        return result;
    }
}