using Application.Features.Schools.Rules;
using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Caching;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Schools.Commands.Schools;

public class ActiveAccessStudentCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest, ICacheRemoverRequest
{
    public UserTypes[] Roles { get; } = [UserTypes.School];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
    public string[] CacheKey { get; } = [$"^{Strings.CacheStatusAndLicence}"];
}

public class ActiveAccessStudentCommandHandler(ISchoolDal schoolDal,
                                               ICommonService commonService) : IRequestHandler<ActiveAccessStudentCommand, bool>
{
    public async Task<bool> Handle(ActiveAccessStudentCommand request, CancellationToken cancellationToken)
    {
        var school = await schoolDal.GetAsync(x => x.Id == commonService.HttpSchoolId, cancellationToken: cancellationToken);
        await SchoolRules.SchoolShouldExists(school);

        school.UpdateUser = commonService.HttpUserId;
        school.UpdateDate = DateTime.Now;
        school.AccessStundents = true;

        await schoolDal.UpdateAsync(school, cancellationToken: cancellationToken);

        return true;
    }
}