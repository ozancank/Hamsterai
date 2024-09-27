using Business.Features.Schools.Rules;
using Business.Features.Users.Rules;
using Business.Services.CommonService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Schools.Commands.Schools;

public class ActiveSchoolCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public int Id { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public string[] HidePropertyNames { get; } = [];
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