using Business.Features.Schools.Models.Teachers;
using Business.Features.Schools.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Schools.Queries.Teachers;

public class GetTeacherByIdQuery : IRequest<GetTeacherModel>, ISecuredRequest<UserTypes>
{
    public byte Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [UserTypes.School];
}

public class GetTeacherByIdQueryHandler(IMapper mapper,
                                        ICommonService commonService,
                                        ITeacherDal teacherDal) : IRequestHandler<GetTeacherByIdQuery, GetTeacherModel>
{
    public async Task<GetTeacherModel> Handle(GetTeacherByIdQuery request, CancellationToken cancellationToken)
    {
        var teacher = await teacherDal.GetAsyncAutoMapper<GetTeacherModel>(
            enableTracking: request.Tracking,
            predicate: x => x.Id == request.Id && x.SchoolId == commonService.HttpSchoolId,
            include: x => x.Include(u => u.School).Include(u => u.TeacherLessons).Include(u => u.TeacherLessons),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await TeacherRules.TeacherShouldExists(teacher);
        return teacher;
    }
}