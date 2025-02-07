using Application.Features.Lessons.Models.Lessons;
using Application.Features.Schools.Rules;
using Application.Features.Students.Rules;
using Application.Features.Teachers.Rules;
using Application.Services.CommonService;
using Domain.Entities;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using static OneOf.Types.TrueFalseOrNull;

namespace Application.Features.Lessons.Queries.Lessons;

public class GetLessonsQuery : IRequest<PageableModel<GetLessonModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetLessonsQueryHandler(IMapper mapper,
                                    ILessonDal lessonDal,
                                    IStudentDal studentDal,
                                    ITeacherDal teacherDal,
                                    IRPackageLessonDal packageLessonDal,
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
                         ? x => x.IsActive && x.RPackageLessons.Any(a => a.IsActive && a.Package != null
                                                                      && a.Package.PackageUsers.Any(p => p.IsActive
                                                                                                      && p.User != null
                                                                                                      && p.User.IsActive
                                                                                                      && p.UserId == commonService.HttpUserId))
                         : x => x.IsActive && x.RPackageLessons.Any(a => a.IsActive
                                                                      && a.Package != null
                                                                      && a.Package.PackageUsers.Any(p => p.IsActive
                                                                                                      && p.User != null
                                                                                                      && p.User.IsActive
                                                                                                      && p.User.School != null
                                                                                                      && p.User.School.IsActive
                                                                                                      && p.User.School.Id == commonService.HttpSchoolId)),
            include: x => x.Include(u => u.TeacherLessons).ThenInclude(u => u.Teacher)
                           .Include(u => u.RPackageLessons).ThenInclude(u => u.Package).ThenInclude(u => u!.PackageUsers).ThenInclude(u => u.User).ThenInclude(u => u != null ? u.School : default),
            orderBy: x => x.OrderBy(x => x.SortNo),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetLessonModel>>(lesson);

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