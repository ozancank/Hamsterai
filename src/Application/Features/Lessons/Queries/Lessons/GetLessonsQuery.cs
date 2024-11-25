using Application.Features.Lessons.Models.Lessons;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Lessons.Queries.Lessons;

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
                       : commonService.HttpUserType == UserTypes.Person
                         ? x => x.IsActive && x.RPackageLessons.Any(a => a.IsActive && a.Package != null && a.Package.PackageUsers.Any(p => p.IsActive && p.User != null && p.User.IsActive && p.UserId == commonService.HttpUserId))
                         : x => x.IsActive && x.RPackageLessons.Any(a => a.IsActive && a.Package != null && a.Package.PackageUsers.Any(p => p.IsActive && p.User != null && p.User.IsActive && p.User.School!=null && p.User.School.IsActive && p.User.School.Id== commonService.HttpSchoolId)),
            include: x => x.Include(u => u.TeacherLessons).ThenInclude(u => u.Teacher)
                           .Include(u => u.RPackageLessons).ThenInclude(u => u.Package).ThenInclude(u => u!.PackageUsers).ThenInclude(u => u.User).ThenInclude(u => u != null ? u.School : default),
            orderBy: x => x.OrderBy(x => x.SortNo),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetLessonModel>>(lesson);
        return result;
    }
}