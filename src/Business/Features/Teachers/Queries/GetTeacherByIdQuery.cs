using Business.Features.Teachers.Models;
using Business.Features.Teachers.Rules;
using Business.Services.CommonService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Teachers.Queries;

public class GetTeacherByIdQuery : IRequest<GetTeacherModel?>, ISecuredRequest<UserTypes>
{
    public int Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [UserTypes.School];
    public bool AllowByPass => false;
}

public class GetTeacherByIdQueryHandler(IMapper mapper,
                                        ICommonService commonService,
                                        IUserDal userDal,
                                        ITeacherDal teacherDal) : IRequestHandler<GetTeacherByIdQuery, GetTeacherModel?>
{
    public async Task<GetTeacherModel?> Handle(GetTeacherByIdQuery request, CancellationToken cancellationToken)
    {
        var teacher = await teacherDal.GetAsyncAutoMapper<GetTeacherModel>(
            enableTracking: request.Tracking,
            predicate: x => x.Id == request.Id && (commonService.HttpUserType == UserTypes.Administator || x.SchoolId == commonService.HttpSchoolId),
            include: x => x.Include(u => u.School).Include(u => u.RTeacherLessons).Include(u => u.RTeacherLessons),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await TeacherRules.TeacherShouldExists(teacher);

        if (teacher != null)
            teacher.UserId = (await userDal.GetAsync(x => x.Type == UserTypes.Teacher && x.ConnectionId == teacher.Id, enableTracking: false, cancellationToken: cancellationToken))?.Id ?? 0;

        return teacher;
    }
}