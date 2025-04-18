﻿using Application.Features.Users.Rules;
using Application.Services.UserService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Caching;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Users.Commands.Users;

public class PassiveUserCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest, ICacheRemoverRequest
{
    public long Id { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.School, UserTypes.Teacher];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
    public string[] CacheKey { get; } = [$"^{Strings.CacheStatusAndLicence}"];
}

public class PassiveUserCommandHandler(IUserDal userDal,
                                       IUserService userService,
                                       UserRules userRules) : IRequestHandler<PassiveUserCommand, bool>
{
    public async Task<bool> Handle(PassiveUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userDal.GetAsync(predicate: userService.GetPredicateForUser(x => x.Id == request.Id), cancellationToken: cancellationToken);
        await UserRules.UserShouldExistsAndActive(user);
        await userRules.UserTypeAllowed(user.Type, user.Id);
        await userRules.UserCanNotPassiveAtAdminUser(user);

        user.IsActive = false;

        await userDal.UpdateAsync(user, cancellationToken: cancellationToken);
        return true;
    }
}