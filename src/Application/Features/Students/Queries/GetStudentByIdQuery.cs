using Application.Features.Students.Models;
using Application.Features.Students.Rules;
using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Students.Queries;

public class GetStudentByIdQuery : IRequest<GetStudentModel?>, ISecuredRequest<UserTypes>
{
    public int Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [UserTypes.School, UserTypes.Teacher];
    public bool AllowByPass => false;
}

public class GetStudentByIdQueryHandler(IMapper mapper,
                                        ICommonService commonService,
                                        IUserDal userDal,
                                        IStudentDal studentDal) : IRequestHandler<GetStudentByIdQuery, GetStudentModel?>
{
    public async Task<GetStudentModel?> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        var student = await studentDal.GetAsyncAutoMapper<GetStudentModel>(
            enableTracking: request.Tracking,
            predicate: x => x.Id == request.Id && (commonService.HttpUserType == UserTypes.Administator || x.ClassRoom!.SchoolId == commonService.HttpSchoolId),
            include: x => x.Include(u => u.ClassRoom).Include(u => u.Teachers),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await StudentRules.StudentShouldExists(student);

        if (student != null)
        {
            var user = await userDal.GetAsync(x => x.Type == UserTypes.Student && x.ConnectionId == student.Id, enableTracking: false, cancellationToken: cancellationToken);
            student.UserId = user?.Id ?? 0;
            student.SchoolId = user?.SchoolId ?? 0;
        }

        return student;
    }
}