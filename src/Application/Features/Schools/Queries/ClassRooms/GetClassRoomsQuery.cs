using Application.Features.Schools.Models.ClassRooms;
using Application.Features.Teachers.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Schools.Queries.ClassRooms;

public class GetClassRoomsQuery : IRequest<PageableModel<GetClassRoomModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School, UserTypes.Teacher];
    public bool AllowByPass => false;
}

public class GetClassRoomsQueryHandler(IMapper mapper,
                                       ICommonService commonService,
                                       ITeacherDal teacherDal,
                                       IClassRoomDal classRoomDal) : IRequestHandler<GetClassRoomsQuery, PageableModel<GetClassRoomModel>>
{
    public async Task<PageableModel<GetClassRoomModel>> Handle(GetClassRoomsQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();

        var classRooms = await classRoomDal.GetPageListAsyncAutoMapper<GetClassRoomModel>(
            enableTracking: false,
            predicate: x => commonService.HttpUserType == UserTypes.Administator || (x.School!.Id == commonService.HttpSchoolId && x.IsActive),
            include: x => x.Include(u => u.Package),
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            orderBy: x => x.OrderBy(x => x.CreateDate),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetClassRoomModel>>(classRooms);

        if (commonService.HttpUserType == UserTypes.Teacher)
        {
            var teacher = await teacherDal.GetAsync(
                predicate: x => x.Id == commonService.HttpConnectionId && x.IsActive && x.RTeacherClassRooms != null && x.RTeacherClassRooms.Count > 0,
                selector: x => new { ClassRoomId = x.RTeacherClassRooms.Select(x => x.ClassRoomId) },
                include: x => x.Include(u => u.RTeacherClassRooms),
                enableTracking: false,
                cancellationToken: cancellationToken);
            await TeacherRules.TeacherShouldExists(teacher);

            result.Items = result.Items.Where(x => teacher.ClassRoomId.Any(a => a == x.Id)).ToList();
        }

        return result;
    }
}