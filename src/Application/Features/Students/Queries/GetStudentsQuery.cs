using Application.Features.Students.Models;
using Application.Features.Teachers.Rules;
using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using DataAccess.EF.Concrete;
using Domain.Entities;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Students.Queries;

public class GetStudentsQuery : IRequest<PageableModel<GetStudentModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School, UserTypes.Teacher];
    public bool AllowByPass => false;
}

public class GetStudentsQueryHandler(IMapper mapper,
                                     ICommonService commonService,
                                     IUserDal userDal,
                                     IStudentDal studentDal) : IRequestHandler<GetStudentsQuery, PageableModel<GetStudentModel>>
{
    public async Task<PageableModel<GetStudentModel>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();

        var schoolId = commonService.HttpSchoolId ?? 0;

        var students = await studentDal.GetPageListAsyncAutoMapper<GetStudentModel>(
            enableTracking: false,
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            predicate: x => commonService.HttpUserType == UserTypes.Administator
                        || (commonService.HttpUserType == UserTypes.School ? x.ClassRoom != null && x.ClassRoom.SchoolId == schoolId
                          : commonService.HttpUserType == UserTypes.Teacher && x.IsActive && x.ClassRoom != null && x.ClassRoom.SchoolId == schoolId && x.ClassRoom.TeacherClassRooms.Any(x => x.TeacherId == commonService.HttpConnectionId)),
            include: x => x.Include(u => u.ClassRoom).ThenInclude(u => u != null ? u.TeacherClassRooms : new HashSet<RTeacherClassRoom>()).ThenInclude(u => u.Teacher),
            orderBy: x => x.OrderBy(x => x.CreateDate),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        var studentIds = students.Items.Select(x => x.Id).ToArray() ?? [];

        var users = await userDal.GetListAsync(
            predicate: x => x.Type == UserTypes.Student && studentIds.Any(s => s == x.ConnectionId),
            selector: x => new { x.Id, x.SchoolId, x.ConnectionId },
            enableTracking: false,
            cancellationToken: cancellationToken);

        students.Items.ForEach(x =>
        {
            var user = users.FirstOrDefault(u => u.ConnectionId == x.Id);
            x.UserId = user?.Id ?? 0;
            x.SchoolId = user?.SchoolId ?? 0;
        }); ;

        var result = mapper.Map<PageableModel<GetStudentModel>>(students);

        //if (commonService.HttpUserType == UserTypes.Teacher)
        //{
        //    var teacher = await teacherDal.GetAsync(
        //        predicate: x => x.Id == commonService.HttpConnectionId
        //                        && x.IsActive
        //                        && x.RTeacherClassRooms != null
        //                        && x.RTeacherClassRooms.Count > 0
        //                        && x.SchoolId == schoolId,
        //        selector: x => new { StudentIds = x.RTeacherClassRooms.Where(x => x.ClassRoom != null && x.ClassRoom.Students != null).SelectMany(c => c.ClassRoom!.Students.Select(s => s.Id)).Distinct() },
        //        cancellationToken: cancellationToken);
        //    await TeacherRules.TeacherShouldExists(teacher);

        //    result.Items = result.Items.Where(x => teacher.StudentIds.Any(s => s == x.Id));
        //}

        return result;
    }
}