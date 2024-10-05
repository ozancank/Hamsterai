using Business.Features.Schools.Rules;
using Business.Features.Users.Rules;
using Business.Services.CommonService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Schools.Commands.Teachers;

public class ActiveTeacherCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public int Id { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School];
    public string[] HidePropertyNames { get; } = [];
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