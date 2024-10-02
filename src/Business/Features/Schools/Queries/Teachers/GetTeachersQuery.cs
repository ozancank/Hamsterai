using Business.Features.Schools.Models.Teachers;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Schools.Queries.Teachers;

public class GetTeachersQuery : IRequest<PageableModel<GetTeacherModel>>, ISecuredRequest<UserTypes>
{
    public PageRequest PageRequest { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School];
}

public class GetTeachersQueryHandler(IMapper mapper,
                                     ICommonService commonService,
                                     ITeacherDal teacherDal) : IRequestHandler<GetTeachersQuery, PageableModel<GetTeacherModel>>
{
    public async Task<PageableModel<GetTeacherModel>> Handle(GetTeachersQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        var schoolId = commonService.HttpSchoolId ?? 0;

        var teachers = await teacherDal.GetPageListAsyncAutoMapper<GetTeacherModel>(
            enableTracking: false,
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            predicate: x => x.SchoolId == schoolId,
            include: x => x.Include(u => u.School).Include(u => u.TeacherLessons).Include(u => u.TeacherLessons),
            orderBy: x => x.OrderBy(x => x.CreateDate),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);
        var result = mapper.Map<PageableModel<GetTeacherModel>>(teachers);
        return result;
    }
}