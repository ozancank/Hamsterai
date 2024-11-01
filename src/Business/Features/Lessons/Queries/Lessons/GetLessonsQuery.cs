using Business.Features.Lessons.Models.Lessons;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Lessons.Queries.Lessons;

public class GetLessonsQuery : IRequest<PageableModel<GetLessonModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetLessonsQueryHandler(IMapper mapper,
                                    ILessonDal lessonDal,
                                    ICommonService commonService) : IRequestHandler<GetLessonsQuery, PageableModel<GetLessonModel>>
{
    public async Task<PageableModel<GetLessonModel>> Handle(GetLessonsQuery request, CancellationToken cancellationToken)
    {

        var lesson = await lessonDal.GetPageListAsyncAutoMapper<GetLessonModel>(
            enableTracking: false,
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            predicate: commonService.HttpUserType == UserTypes.Administator
                       ? x => x.IsActive
                       : x => x.IsActive && x.RPackageLessons.Any(a => a.Package!.RPackageSchools.Any(b => b.School!.Id == commonService.HttpSchoolId)),
            include: x => x.Include(u => u.TeacherLessons).ThenInclude(u => u.Teacher)
                           .Include(u => u.RPackageLessons).ThenInclude(u => u.Package).ThenInclude(u => u!.RPackageSchools).ThenInclude(u => u.School),
            orderBy: x => x.OrderBy(x => x.SortNo),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetLessonModel>>(lesson);
        return result;
    }
}