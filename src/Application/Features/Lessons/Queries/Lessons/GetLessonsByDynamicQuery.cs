using Application.Features.Lessons.Models.Lessons;
using Application.Features.Students.Rules;
using Application.Features.Teachers.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Lessons.Queries.Lessons;

public class GetLessonsByDynamicQuery : IRequest<PageableModel<GetLessonModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required Dynamic Dynamic { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetLessonsByDynamicQueryHandler(IMapper mapper,
                                             ICommonService commonService,
                                             IStudentDal studentDal,
                                             ITeacherDal teacherDal,
                                             IRPackageLessonDal packageLessonDal,
                                             ILessonDal lessonDal) : IRequestHandler<GetLessonsByDynamicQuery, PageableModel<GetLessonModel>>
{
    public async Task<PageableModel<GetLessonModel>> Handle(GetLessonsByDynamicQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.Dynamic ??= new Dynamic();

        var users = await lessonDal.GetPageListAsyncAutoMapperByDynamic<GetLessonModel>(
            dynamic: request.Dynamic,
            enableTracking: false,
            index: request.PageRequest.Page,
            size: request.PageRequest.PageSize,
            include: x => x.Include(u => u.TeacherLessons).ThenInclude(u => u.Teacher)
                           .Include(u => u.RPackageLessons).ThenInclude(u => u.Package),
            defaultOrderColumnName: x => x.SortNo,
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);
        var result = mapper.Map<PageableModel<GetLessonModel>>(users);

        if (commonService.HttpUserType == UserTypes.Student)
        {
            var student = await studentDal.GetAsync(
                predicate: x => x.Id == commonService.HttpConnectionId && x.IsActive && x.ClassRoom != null && x.ClassRoom.IsActive,
                selector: x => new { PackageId = x.ClassRoom != null ? x.ClassRoom.PackageId : 0 },
                include: x => x.Include(u => u.ClassRoom),
                enableTracking: false,
                cancellationToken: cancellationToken);
            await StudentRules.StudentShouldExists(student);

            var lessonIds = await packageLessonDal.GetListAsync(
                predicate: x => x.PackageId == student.PackageId && x.IsActive,
            selector: x => new { x.LessonId },
            enableTracking: false,
            cancellationToken: cancellationToken);

            result.Items = [.. result.Items.Where(x => lessonIds.Any(a => a.LessonId == x.Id))];
        }

        if (commonService.HttpUserType == UserTypes.Teacher)
        {
            var teacher = await teacherDal.GetAsync(
                predicate: x => x.Id == commonService.HttpConnectionId && x.IsActive && x.RTeacherLessons != null && x.RTeacherLessons.Count > 0,
                selector: x => new { LessonIds = x.RTeacherLessons.Where(l => l.IsActive).Select(l => l.LessonId) },
                include: x => x.Include(u => u.RTeacherLessons).ThenInclude(u => u.Lesson),
                enableTracking: false,
                cancellationToken: cancellationToken);
            await TeacherRules.TeacherShouldExists(teacher);

            result.Items = [.. result.Items.Where(x => teacher.LessonIds.Any(a => a == x.Id))];
        }

        return result;
    }
}