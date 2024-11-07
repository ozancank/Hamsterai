using Business.Features.Schools.Models.Schools;
using Business.Features.Schools.Rules;
using Business.Services.CommonService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Schools.Queries.Schools;

public class GetSchoolDashboardQuery : IRequest<GetSchoolDashboardModel>, ISecuredRequest<UserTypes>
{
    public int Id { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School];
    public bool AllowByPass => false;
}

public class GetSchoolDashboardQueryHandler(ISchoolDal schoolDal,
                                            IUserDal userDal,
                                            ICommonService commonService,
                                            IQuestionDal questionDal,
                                            ISimilarDal similarQuestionDal,
                                            IHomeworkDal homeworkDal,
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

        var result = new GetSchoolDashboardModel();

        var users = await userDal.GetListAsync(
            enableTracking: false,
            predicate: x => x.SchoolId == commonService.HttpSchoolId && x.IsActive,
            selector: x => new
            {
                x.Type,
            },
            cancellationToken: cancellationToken);

        result.SchoolId = school.Id;
        result.UserId = commonService.HttpUserId;
        result.SchoolName = school.Name;
        result.LicenceEndDate = school.LicenseEndDate;
        result.RemainingDay = (school.LicenseEndDate - DateTime.Now).Days;
        result.MaxUserCount = school.UserCount;
        result.TotalTeacherCount = users.Sum(x => x.Type == UserTypes.Teacher ? 1 : 0);
        result.TotalStudentCount = users.Sum(x => x.Type == UserTypes.Student ? 1 : 0);
        result.TotalClassRoomCount = await classRoomDal.CountOfRecordAsync(enableTracking: false, predicate: x => x.IsActive && x.SchoolId == school.Id, cancellationToken: cancellationToken);
        result.SendQuestionCount = await questionDal.CountOfRecordAsync(enableTracking: false, predicate: x => x.User!.IsActive && x.User.SchoolId == school.Id, include: x => x.Include(u => u.User), cancellationToken: cancellationToken);
        result.SendSimilarCount = await similarQuestionDal.CountOfRecordAsync(enableTracking: false, predicate: x => x.User!.IsActive && x.User.SchoolId == school.Id, include: x => x.Include(u => u.User), cancellationToken: cancellationToken);
        result.TotalHomeworkCount = await homeworkDal.CountOfRecordAsync(enableTracking: false, predicate: x => x.User!.IsActive && x.User.SchoolId == school.Id, include: x => x.Include(u => u.User), cancellationToken: cancellationToken);

        result.SendQuestionByLesson = await questionDal.Query().AsNoTracking()
                                                       .Include(x => x.Lesson)
                                                       .Include(x => x.User)
                                                       .Where(x => x.User!.IsActive && x.Lesson!.IsActive && x.User.SchoolId == school.Id)
                                                       .GroupBy(x => x.Lesson!.Name)
                                                       .Select(x => new { x.Key, Count = x.Count() })
                                                       .ToDictionaryAsync(x => x.Key!, x => x.Count, cancellationToken);
        result.SendQuestionByPackage = await questionDal.Query().AsNoTracking()
                                                        .Include(x => x.Lesson).ThenInclude(x => x!.RPackageLessons).ThenInclude(x => x.Package)
                                                        .Include(x => x.User)
                                                        .Where(x => x.User!.IsActive && x.Lesson!.IsActive && x.Lesson.RPackageLessons.Any(g => g.IsActive && g.Package!.IsActive) && x.User.SchoolId == school.Id)
                                                        .SelectMany(x => x.Lesson!.RPackageLessons.Where(g => g.IsActive && g.Package!.IsActive))
                                                        .GroupBy(x => x.Package!.Name)
                                                        .Select(x => new { x.Key, Count = x.Count() })
                                                        .ToDictionaryAsync(x => x.Key!, x => x.Count, cancellationToken);
        result.SendQuestionByClassRoom = await questionDal.Query().AsNoTracking()
                                                          .Include(x => x.User).ThenInclude(x => x!.School).ThenInclude(x => x!.ClassRooms).ThenInclude(x => x.Students)
                                                          .Where(x => x.User!.IsActive && x.User.School!.Id == school.Id && x.User.Type == UserTypes.Student)
                                                          .Where(x => x.User!.School!.ClassRooms.Any(c => c.IsActive && c.Students.Any(s => s.IsActive && s.Id == x.User.ConnectionId)))
                                                          .SelectMany(x => x.User!.School!.ClassRooms.Where(c => c.IsActive && c.Students.Any(s => s.IsActive && s.Id == x.User.ConnectionId)))
                                                          .GroupBy(c => new { c.No, c.Branch })
                                                          .Select(x => new { Key = $"{x.Key.No}-{x.Key.Branch}", Count = x.Count() })
                                                          .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken: cancellationToken);
        result.SendQuestionByGain = await questionDal.Query().AsNoTracking()
                                                     .Include(x => x.Gain)
                                                     .Include(x => x.User).ThenInclude(x => x!.School)
                                                     .Where(x => x.User!.IsActive && x.User.School!.Id == school.Id && x.User.Type == UserTypes.Student && x.Gain != null)
                                                     .GroupBy(x => x.Gain!.Name)
                                                     .Select(x => new { x.Key, Count = x.Count() })
                                                     .ToDictionaryAsync(x => x.Key!, x => x.Count, cancellationToken);

        result.SendQuestionByName = await questionDal.Query().AsNoTracking()
                                                     .Include(x => x.User).ThenInclude(x => x!.School)
                                                     .Where(x => x.User!.IsActive && x.User.School!.Id == school.Id && x.User.Type == UserTypes.Student)
                                                     .GroupBy(x => new { x.User!.Name, x.User.Surname })
                                                     .Select(x => new { Key = $"{x.Key.Name} {x.Key.Surname}", Count = x.Count() })
                                                     .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken);

        result.SendQuestionByDay = (await questionDal.Query().AsNoTracking()
                                                            .Include(x => x.User).ThenInclude(x => x!.School)
                                                            .Where(x => x.User!.IsActive && x.User.School!.Id == school.Id && x.User.Type == UserTypes.Student)
                                                            .GroupBy(x => x.CreateDate.Date)
                                                            .Select(x => new { x.Key, Count = x.Count() })
                                                            .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken))
                                   .GroupBy(x => x.Key.ToStringDayOfWeek()).ToDictionary(x => x.Key, x => x.Sum(s => s.Value));

        result.SendSimilarByLesson = await similarQuestionDal.Query().AsNoTracking()
                                                             .Include(x => x.Lesson)
                                                             .Include(x => x.User)
                                                             .Where(x => x.User!.IsActive && x.Lesson!.IsActive && x.User.SchoolId == school.Id)
                                                             .GroupBy(x => x.Lesson!.Name)
                                                             .Select(x => new { x.Key, Count = x.Count() })
                                                             .ToDictionaryAsync(x => x.Key!, x => x.Count, cancellationToken);
        result.SendSimilarByPackage = await similarQuestionDal.Query().AsNoTracking()
                                                            .Include(x => x.Lesson).ThenInclude(x => x!.RPackageLessons).ThenInclude(x => x.Package)
                                                            .Include(x => x.User)
                                                            .Where(x => x.User!.IsActive && x.Lesson!.IsActive && x.Lesson.RPackageLessons.Any(g => g.IsActive && g.Package!.IsActive) && x.User.SchoolId == school.Id)
                                                            .SelectMany(x => x.Lesson!.RPackageLessons.Where(g => g.IsActive && g.Package!.IsActive))
                                                            .GroupBy(x => x.Package!.Name)
                                                            .Select(x => new { x.Key, Count = x.Count() })
                                                            .ToDictionaryAsync(x => x.Key!, x => x.Count, cancellationToken);
        result.SendSimilarByClassRoom = await similarQuestionDal.Query().AsNoTracking()
                                                                .Include(x => x.User).ThenInclude(x => x!.School).ThenInclude(x => x!.ClassRooms).ThenInclude(x => x.Students)
                                                                .Where(x => x.User!.IsActive && x.User.School!.Id == school.Id && x.User.Type == UserTypes.Student)
                                                                .Where(x => x.User!.School!.ClassRooms.Any(c => c.IsActive && c.Students.Any(s => s.IsActive && s.Id == x.User.ConnectionId)))
                                                                .SelectMany(x => x.User!.School!.ClassRooms.Where(c => c.IsActive && c.Students.Any(s => s.IsActive && s.Id == x.User.ConnectionId)))
                                                                .GroupBy(c => new { c.No, c.Branch })
                                                                .Select(x => new { Key = $"{x.Key.No}-{x.Key.Branch}", Count = x.Count() })
                                                                .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken: cancellationToken);
        result.SendSimilarByGain = await similarQuestionDal.Query().AsNoTracking()
                                                           .Include(x => x.Gain)
                                                           .Include(x => x.User).ThenInclude(x => x!.School)
                                                           .Where(x => x.User!.IsActive && x.User.School!.Id == school.Id && x.User.Type == UserTypes.Student && x.Gain != null)
                                                           .GroupBy(x => x.Gain!.Name)
                                                           .Select(x => new { x.Key, Count = x.Count() })
                                                           .ToDictionaryAsync(x => x.Key!, x => x.Count, cancellationToken);
        result.SendSimilarByName = await similarQuestionDal.Query().AsNoTracking()
                                                           .Include(x => x.User).ThenInclude(x => x!.School)
                                                           .Where(x => x.User!.IsActive && x.User.School!.Id == school.Id && x.User.Type == UserTypes.Student)
                                                           .GroupBy(x => new { x.User!.Name, x.User.Surname })
                                                           .Select(x => new { Key = $"{x.Key.Name} {x.Key.Surname}", Count = x.Count() })
                                                           .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken);
        result.SendSimilarByDay = (await similarQuestionDal.Query().AsNoTracking()
                                                           .Include(x => x.User).ThenInclude(x => x!.School)
                                                           .Where(x => x.User!.IsActive && x.User.School!.Id == school.Id && x.User.Type == UserTypes.Student)
                                                           .GroupBy(x => x.CreateDate.Date)
                                                           .Select(x => new { x.Key, Count = x.Count() })
                                                           .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken))
                                  .GroupBy(x => x.Key.ToStringDayOfWeek()).ToDictionary(x => x.Key, x => x.Sum(s => s.Value));

        result.HomeworkByTeacher = await homeworkDal.Query().AsNoTracking()
                                                    .Include(x => x.User).ThenInclude(x => x!.School)
                                                    .Where(x => x.User!.IsActive && x.User.SchoolId == school.Id && x.User.Type == UserTypes.Teacher)
                                                    .GroupBy(x => new { x.User!.Name, x.User.Surname })
                                                    .Select(x => new { Key = $"{x.Key.Name} {x.Key.Surname}", Count = x.Count() })
                                                    .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken);

        result.HomeworkByLesson = await homeworkDal.Query().AsNoTracking()
                                                   .Include(x => x.Lesson)
                                                   .Include(x => x.User)
                                                   .Where(x => x.User!.IsActive && x.Lesson!.IsActive && x.User.SchoolId == school.Id)
                                                   .GroupBy(x => x.Lesson!.Name)
                                                   .Select(x => new { x.Key, Count = x.Count() })
                                                   .ToDictionaryAsync(x => x.Key!, x => x.Count, cancellationToken);

        //result

        result.TotalUserCount = result.TotalTeacherCount + result.TotalStudentCount;
        result.TotalQuestionCount = result.SendQuestionCount + result.SendSimilarCount;
        result.TotalQuestionByLesson = result.SendQuestionByLesson.Concat(result.SendSimilarByLesson).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum(s => s.Value));
        result.TotalQuestionByPackage = result.SendQuestionByPackage.Concat(result.SendSimilarByPackage).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum(s => s.Value));
        result.TotalQuestionByClassRoom = result.SendQuestionByClassRoom.Concat(result.SendSimilarByClassRoom).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum(s => s.Value));
        result.TotalQuestionByGain = result.SendQuestionByGain.Concat(result.SendSimilarByGain).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum(s => s.Value));
        result.TotalQuestionByName = result.SendQuestionByName.Concat(result.SendSimilarByName).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum(s => s.Value));

        return result;
    }
}