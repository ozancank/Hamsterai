using Application.Features.Schools.Rules;
using Application.Features.Users.Rules;
using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Caching;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Schools.Commands.Schools;

public class ActiveSchoolCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest, ICacheRemoverRequest
{
    public int Id { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
    public string[] CacheKey { get; } = [$"^{Strings.CacheStatusAndLicence}"];
}

public class ActiveSchoolCommandHandler(ISchoolDal schoolDal,
                                        IUserDal userDal,
                                        ICommonService commonService) : IRequestHandler<ActiveSchoolCommand, bool>
{
    public async Task<bool> Handle(ActiveSchoolCommand request, CancellationToken cancellationToken)
    {
        var school = await schoolDal.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        await SchoolRules.SchoolShouldExists(school);

        var user = await userDal.GetAsync(x => x.SchoolId == school.Id && x.Type == UserTypes.School, cancellationToken: cancellationToken);
        await UserRules.UserShouldExists(user);

        school.UpdateUser = commonService.HttpUserId;
        school.UpdateDate = DateTime.Now;
        school.IsActive = true;

        user.IsActive = true;

        await schoolDal.ExecuteWithTransactionAsync(async () =>
        {
            await schoolDal.UpdateAsync(school, cancellationToken: cancellationToken);
            await userDal.UpdateAsync(user, cancellationToken: cancellationToken);
        }, cancellationToken: cancellationToken);

        return true;
    }
}