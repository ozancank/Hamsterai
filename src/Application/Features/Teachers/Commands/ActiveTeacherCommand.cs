using Application.Features.Teachers.Rules;
using Application.Features.Users.Rules;
using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Caching;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Teachers.Commands;

public class ActiveTeacherCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest, ICacheRemoverRequest
{
    public int Id { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
    public string[] CacheKey { get; } = [$"^{Strings.CacheStatusAndLicence}"];
}

public class ActiveTeacherCommandHandler(ITeacherDal teacherDal,
                                        IUserDal userDal,
                                        ICommonService commonService) : IRequestHandler<ActiveTeacherCommand, bool>
{
    public async Task<bool> Handle(ActiveTeacherCommand request, CancellationToken cancellationToken)
    {
        var teacher = await teacherDal.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        await TeacherRules.TeacherShouldExists(teacher);

        var user = await userDal.GetAsync(x => x.ConnectionId == teacher.Id && x.Type == UserTypes.Teacher, cancellationToken: cancellationToken);
        await UserRules.UserShouldExists(user);

        teacher.UpdateUser = commonService.HttpUserId;
        teacher.UpdateDate = DateTime.Now;
        teacher.IsActive = true;

        user.IsActive = true;

        await teacherDal.ExecuteWithTransactionAsync(async () =>
        {
            await teacherDal.UpdateAsync(teacher, cancellationToken: cancellationToken);
            await userDal.UpdateAsync(user, cancellationToken: cancellationToken);
        }, cancellationToken: cancellationToken);

        return true;
    }
}