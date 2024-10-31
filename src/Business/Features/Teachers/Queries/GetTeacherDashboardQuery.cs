using Business.Features.Teachers.Models;
using Business.Features.Teachers.Rules;
using Business.Services.CommonService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Teachers.Queries;

public class GetTeacherDashboardQuery : IRequest<GetTeacherDashboardModel>, ISecuredRequest<UserTypes>
{
    public byte Id { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Teacher];
}

public class GetTeacherDashboardQueryHandler(ITeacherDal teacherDal,
                                             IUserDal userDal,
                                             ICommonService commonService,
                                             IQuestionDal questionDal,
                                             ISimilarDal similarQuestionDal) : IRequestHandler<GetTeacherDashboardQuery, GetTeacherDashboardModel>
{
    public async Task<GetTeacherDashboardModel> Handle(GetTeacherDashboardQuery request, CancellationToken cancellationToken)
    {
        var result = new GetTeacherDashboardModel();

        var users = await userDal.GetListAsync(
            enableTracking: false,
            predicate: x => x.SchoolId == commonService.HttpSchoolId.GetValueOrDefault() && x.IsActive,
            selector: x => new
            {
                x.Id,
                x.Type,
                x.ConnectionId,
            },
            cancellationToken: cancellationToken);

        if(users.Count == 0) return result;

        var teacherId = users.Find(x => x.Id == commonService.HttpUserId && x.Type == UserTypes.Teacher)!.ConnectionId;

        var teacher = await teacherDal.GetAsync(
            enableTracking: false,
            predicate: x => x.Id == teacherId && x.IsActive,
            include: x => x.Include(u => u.RTeacherLessons).ThenInclude(u => u.Lesson)
                           .Include(u => u.School)
                           .Include(u => u.RTeacherClassRooms).ThenInclude(u => u.ClassRoom).ThenInclude(u => u!.Students),
            selector: x => new
            {
                x.Id,
                x.Name,
                x.Surname,
                x.SchoolId,
                x.RTeacherLessons,
                x.RTeacherClassRooms,
                x.School,
            },
            cancellationToken: cancellationToken);

        await TeacherRules.TeacherShouldExists(teacher);

        result.TeacherId = teacher.Id;
        result.UserId = commonService.HttpUserId;
        result.SchoolId = teacher.SchoolId;
        result.SchoolName = teacher.School!.Name;
        result.TeacherName = $"{teacher.Name} {teacher.Surname}";
        result.TotalStudentCount = teacher.RTeacherClassRooms.Sum(x => x.ClassRoom!.Students.Count);
        result.TotalClassRoomCount = teacher.RTeacherClassRooms.Count;
        result.TotalLessonCount = teacher.RTeacherLessons.Count;
        result.SendQuestionCount = await questionDal.Query().AsNoTracking()
                                                    .Include(x => x.User).ThenInclude(u => u!.School).ThenInclude(u => u!.Teachers)
                                                    .Where(x => x.User!.IsActive && x.User.SchoolId == teacher.SchoolId && x.User.Type == UserTypes.Student)
                                                    .Where(x => x.User!.School!.Teachers.Where(t => t.Id == teacherId)
                                                                 .SelectMany(t => t.RTeacherClassRooms)
                                                                 .SelectMany(tc => tc.ClassRoom!.Students)
                                                                 .Any(s => x.User.ConnectionId == s.Id))
                                                    .CountAsync(cancellationToken: cancellationToken);
        result.SendSimilarCount = await similarQuestionDal.Query().AsNoTracking()
                                                          .Include(x => x.User).ThenInclude(u => u!.School).ThenInclude(u => u!.Teachers)
                                                          .Where(x => x.User!.IsActive && x.User.SchoolId == teacher.SchoolId && x.User.Type == UserTypes.Student)
                                                          .Where(x => x.User!.School!.Teachers.Where(t => t.Id == teacherId)
                                                                       .SelectMany(t => t.RTeacherClassRooms)
                                                                       .SelectMany(tc => tc.ClassRoom!.Students)
                                                                       .Any(s => x.User.ConnectionId == s.Id))
                                                          .CountAsync(cancellationToken: cancellationToken);
        result.SendQuestionByLesson = await questionDal.Query().AsNoTracking()
                                                       .Include(x => x.Lesson).ThenInclude(x => x!.TeacherLessons)
                                                       .Include(x => x.User).ThenInclude(u => u!.School).ThenInclude(u => u!.Teachers).ThenInclude(u => u.RTeacherClassRooms).ThenInclude(u => u.ClassRoom).ThenInclude(u => u!.Students)
                                                       .Where(x => x.User!.IsActive && x.User.SchoolId == teacher.SchoolId && x.User.Type == UserTypes.Student)
                                                       .Where(x => x.User!.School!.Teachers.Where(t => t.Id == teacherId)
                                                                    .SelectMany(t => t.RTeacherClassRooms)
                                                                    .SelectMany(tc => tc.ClassRoom!.Students)
                                                                    .Any(s => x.User.ConnectionId == s.Id))
                                                       .Where(x => x.Lesson!.IsActive && x.Lesson.TeacherLessons.Any(t => t.IsActive && t.TeacherId == teacher.Id))
                                                       .GroupBy(x => x.Lesson!.Name)
                                                       .ToDictionaryAsync(x => x.Key!, x => x.Count(), cancellationToken);
        result.SendQuestionByPackage = await questionDal.Query().AsNoTracking()
                                                      .Include(x => x.Lesson).ThenInclude(x => x!.TeacherLessons).ThenInclude(x => x.Lesson)
                                                      .Include(x => x.User).ThenInclude(u => u!.School).ThenInclude(u => u!.Teachers).ThenInclude(u => u.RTeacherClassRooms).ThenInclude(u => u.ClassRoom).ThenInclude(u => u!.Students)
                                                      .Where(x => x.User!.IsActive && x.User.SchoolId == teacher.SchoolId && x.User.Type == UserTypes.Student)
                                                      .Where(x => x.User!.School!.Teachers.Where(t => t.Id == teacherId)
                                                                     .SelectMany(t => t.RTeacherClassRooms)
                                                                     .SelectMany(tc => tc.ClassRoom!.Students)
                                                                     .Any(s => x.User.ConnectionId == s.Id))
                                                      .SelectMany(x => x.Lesson!.RPackageLessons.Where(g => g.IsActive && g.Package!.IsActive))
                                                      .GroupBy(x => x.Package!.Name)
                                                      .ToDictionaryAsync(x => x.Key!, x => x.Count(), cancellationToken);
        result.SendQuestionByClassRoom = await questionDal.Query().AsNoTracking()
                                                          .Include(x => x.Lesson).ThenInclude(x => x!.TeacherLessons).ThenInclude(x => x.Lesson)
                                                          .Include(x => x.User).ThenInclude(u => u!.School).ThenInclude(u => u!.Teachers).ThenInclude(u => u.RTeacherClassRooms).ThenInclude(u => u.ClassRoom).ThenInclude(u => u!.Students)
                                                          .Where(x => x.User!.IsActive && x.User.SchoolId == teacher.SchoolId && x.User.Type == UserTypes.Student)
                                                          .Where(x => x.User!.School!.Teachers.Where(t => t.Id == teacherId)
                                                                         .SelectMany(t => t.RTeacherClassRooms)
                                                                         .SelectMany(tc => tc.ClassRoom!.Students)
                                                                         .Any(s => x.User.ConnectionId == s.Id))
                                                          .SelectMany(x => x.User!.School!.ClassRooms.Where(c => c.IsActive && c.Students.Any(s => s.IsActive && s.Id == x.User.ConnectionId)))
                                                          .GroupBy(c => new { c.No, c.Branch })
                                                          .Select(x => new { Key = $"{x.Key.No}-{x.Key.Branch}", Count = x.Count() })
                                                          .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken: cancellationToken);

        result.SendSimilarByLesson = await similarQuestionDal.Query().AsNoTracking()
                                                             .Include(x => x.Lesson).ThenInclude(x => x!.TeacherLessons)
                                                             .Include(x => x.User).ThenInclude(u => u!.School).ThenInclude(u => u!.Teachers).ThenInclude(u => u.RTeacherClassRooms).ThenInclude(u => u.ClassRoom).ThenInclude(u => u!.Students)
                                                             .Where(x => x.User!.IsActive && x.User.SchoolId == teacher.SchoolId && x.User.Type == UserTypes.Student)
                                                             .Where(x => x.User!.School!.Teachers.Where(t => t.Id == teacherId)
                                                                          .SelectMany(t => t.RTeacherClassRooms)
                                                                          .SelectMany(tc => tc.ClassRoom!.Students)
                                                                          .Any(s => x.User.ConnectionId == s.Id))
                                                             .Where(x => x.Lesson!.IsActive && x.Lesson.TeacherLessons.Any(t => t.IsActive && t.TeacherId == teacher.Id))
                                                             .GroupBy(x => x.Lesson!.Name)
                                                             .ToDictionaryAsync(x => x.Key!, x => x.Count(), cancellationToken);
        result.SendSimilarByPackage = await similarQuestionDal.Query().AsNoTracking()
                                                            .Include(x => x.Lesson).ThenInclude(x => x!.TeacherLessons).ThenInclude(x => x.Lesson)
                                                            .Include(x => x.User).ThenInclude(u => u!.School).ThenInclude(u => u!.Teachers).ThenInclude(u => u.RTeacherClassRooms).ThenInclude(u => u.ClassRoom).ThenInclude(u => u!.Students)
                                                            .Where(x => x.User!.IsActive && x.User.SchoolId == teacher.SchoolId && x.User.Type == UserTypes.Student)
                                                            .Where(x => x.User!.School!.Teachers.Where(t => t.Id == teacherId)
                                                                           .SelectMany(t => t.RTeacherClassRooms)
                                                                           .SelectMany(tc => tc.ClassRoom!.Students)
                                                                           .Any(s => x.User.ConnectionId == s.Id))
                                                            .SelectMany(x => x.Lesson!.RPackageLessons.Where(g => g.IsActive && g.Package!.IsActive))
                                                            .GroupBy(x => x.Package!.Name)
                                                            .ToDictionaryAsync(x => x.Key!, x => x.Count(), cancellationToken);
        result.SendSimilarByClassRoom = await similarQuestionDal.Query().AsNoTracking()
                                                                .Include(x => x.Lesson).ThenInclude(x => x!.TeacherLessons).ThenInclude(x => x.Lesson)
                                                                .Include(x => x.User).ThenInclude(u => u!.School).ThenInclude(u => u!.Teachers).ThenInclude(u => u.RTeacherClassRooms).ThenInclude(u => u.ClassRoom).ThenInclude(u => u!.Students)
                                                                .Where(x => x.User!.IsActive && x.User.SchoolId == teacher.SchoolId && x.User.Type == UserTypes.Student)
                                                                .Where(x => x.User!.School!.Teachers.Where(t => t.Id == teacherId)
                                                                               .SelectMany(t => t.RTeacherClassRooms)
                                                                               .SelectMany(tc => tc.ClassRoom!.Students)
                                                                               .Any(s => x.User.ConnectionId == s.Id))
                                                                .SelectMany(x => x.User!.School!.ClassRooms.Where(c => c.IsActive && c.Students.Any(s => s.IsActive && s.Id == x.User.ConnectionId)))
                                                                .GroupBy(c => new { c.No, c.Branch })
                                                                .Select(x => new { Key = $"{x.Key.No}-{x.Key.Branch}", Count = x.Count() })
                                                                .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken: cancellationToken);

        result.TotalQuestionCount = result.SendQuestionCount + result.SendSimilarCount;
        result.TotalQuestionByLesson = result.SendQuestionByLesson.Concat(result.SendSimilarByLesson).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum(s => s.Value));
        result.TotalQuestionByPackage = result.SendQuestionByPackage.Concat(result.SendSimilarByPackage).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum(s => s.Value));
        result.TotalQuestionByClassRoom = result.SendQuestionByClassRoom.Concat(result.SendSimilarByClassRoom).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum(s => s.Value));

        return result;
    }
}