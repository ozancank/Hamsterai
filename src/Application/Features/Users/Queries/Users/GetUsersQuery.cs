﻿using Application.Features.Users.Models.User;
using Application.Services.UserService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Users.Queries.Users;

public class GetUsersQuery : IRequest<PageableModel<GetUserModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.School, UserTypes.Teacher];
    public bool AllowByPass => false;
}

public class GetUsersQueryHandler(IMapper mapper,
                                  IUserDal userDal,
                                  IUserService userService) : IRequestHandler<GetUsersQuery, PageableModel<GetUserModel>>
{
    public async Task<PageableModel<GetUserModel>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();

        var users = await userDal.GetPageListAsyncAutoMapper<GetUserModel>(
            predicate: userService.GetPredicateForUser(),
            orderBy: x => x.OrderBy(o => o.Name),
            enableTracking: false,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken
            );

        var result = mapper.Map<PageableModel<GetUserModel>>(users);
        return result;
    }
}