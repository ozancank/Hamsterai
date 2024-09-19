using Business.Features.Schools.Models.Schools;
using Business.Features.Schools.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Schools.Queries.Schools;

public class GetSchoolDashboardQuery : IRequest<GetSchoolDashboardModel>, ISecuredRequest<UserTypes>
{
    public byte Id { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School];
}

public class GetSchoolDashboardQueryHandler(ISchoolDal schoolDal,
                                            IUserDal userDal,
                                            ICommonService commonService,
                                            IQuestionDal questionDal,
                                            ISimilarQuestionDal similarQuestionDal,
                                            IClassRoomDal classRoomDal) : IRequestHandler<GetSchoolDashboardQuery, GetSchoolDashboardModel>
{
    public async Task<GetSchoolDashboardModel> Handle(GetSchoolDashboardQuery request, CancellationToken cancellationToken)
    {
        var school = await schoolDal.GetAsync(
            enableTracking: false,
            predicate: x => x.Id == commonService.HttpSchoolId.GetValueOrDefault() && x.IsActive,
            selector: x => new
            {
                x.Id,
                x.Name,
                x.UserCount,
                x.LicenseEndDate,
            },
            cancellationToken: cancellationToken);

        await SchoolRules.SchoolShouldExists(school);

        var users = await userDal.GetListAsync(
            enableTracking: false,
            predicate: x => x.SchoolId == commonService.HttpSchoolId && x.IsActive,
            selector: x => new
            {
                x.Type,
            },
            cancellationToken: cancellationToken);

        var result = new GetSchoolDashboardModel();
        result.SchoolId = commonService.HttpSchoolId.Value;
        result.UserId = commonService.HttpUserId;
        result.SchoolName = school.Name;
        result.LicenceEndDate = school.LicenseEndDate;
        result.RemainingDay = (school.LicenseEndDate - DateTime.Now).Days;
        result.MaxUserCount = school.UserCount;
        result.TotalTeacherCount = users.Sum(x => x.Type == UserTypes.Teacher ? 1 : 0);
        result.TotalStudentCount = users.Sum(x => x.Type == UserTypes.Student ? 1 : 0);
        result.TotalClassRoomCount = await classRoomDal.CountOfRecordAsync(enableTracking: false, predicate: x => x.IsActive && x.SchoolId == school.Id, cancellationToken: cancellationToken);
        result.SendQuestionCount = await questionDal.CountOfRecordAsync(enableTracking: false, predicate: x => x.User.IsActive && x.User.SchoolId == school.Id, include: x => x.Include(u => u.User), cancellationToken: cancellationToken);
        result.SendSimilarCount = await similarQuestionDal.CountOfRecordAsync(enableTracking: false, predicate: x => x.User.IsActive && x.User.SchoolId == school.Id, include: x => x.Include(u => u.User), cancellationToken: cancellationToken);
        result.SendQuestionByLesson = await questionDal.Query().AsNoTracking()
                                                       .Include(x => x.Lesson)
                                                       .Include(x => x.User)
                                                       .Where(x => x.User.IsActive && x.Lesson.IsActive && x.User.SchoolId == school.Id)
                                                       .GroupBy(x => x.Lesson.Name)
                                                       .Select(x => new { x.Key, Count = x.Count() })
                                                       .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken);
        result.SendQuestionByGroup = await questionDal.Query().AsNoTracking()
                                                      .Include(x => x.Lesson).ThenInclude(x => x.LessonGroups).ThenInclude(x => x.Group)
                                                      .Include(x => x.User)
                                                      .Where(x => x.User.IsActive && x.Lesson.IsActive && x.Lesson.LessonGroups.Any(g => g.IsActive && g.Group.IsActive) && x.User.SchoolId == school.Id)
                                                      .SelectMany(x => x.Lesson.LessonGroups.Where(g => g.IsActive && g.Group.IsActive))
                                                      .GroupBy(x => x.Group.Name)
                                                      .Select(x => new { x.Key, Count = x.Count() })
                                                      .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken);

        result.SendQuestionByClassRoom = await questionDal.Query().AsNoTracking()
                                                          .Include(x => x.User).ThenInclude(x => x.School).ThenInclude(x => x.ClassRooms).ThenInclude(x => x.Students)
                                                          .Where(x => x.User.IsActive && x.User.School.Id == school.Id && x.User.Type == UserTypes.Student)
                                                          .Where(x => x.User.School.ClassRooms.Any(c => c.IsActive && c.Students.Any(s => s.IsActive && s.Id == x.User.ConnectionId)))
                                                          .SelectMany(x => x.User.School.ClassRooms.Where(c => c.IsActive && c.Students.Any(s => s.IsActive && s.Id == x.User.ConnectionId)))
                                                          .GroupBy(c => new { c.No, c.Branch })
                                                          .Select(x => new { Key = $"{x.Key.No}-{x.Key.Branch}", Count = x.Count() })
                                                          .ToDictionaryAsync(x => x.Key, x => x.Count);

        result.TotalUserCount = result.TotalTeacherCount + result.TotalStudentCount;
        result.TotalQuestionCount = result.SendQuestionCount + result.SendSimilarCount;




        return result;
    }
}