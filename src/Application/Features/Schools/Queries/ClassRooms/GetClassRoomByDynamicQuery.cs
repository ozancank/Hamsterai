using Application.Features.Schools.Models.ClassRooms;
using Application.Features.Teachers.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Schools.Queries.ClassRooms;

public class GetClassRoomsByDynamicQuery : IRequest<PageableModel<GetClassRoomModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required Dynamic Dynamic { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School, UserTypes.Teacher];
    public bool AllowByPass => false;
}

public class GetClassRoomsByDynamicQueryHandler(IMapper mapper,
                                                ICommonService commonService,
                                                ITeacherDal teacherDal,
                                                IClassRoomDal classRoomDal) : IRequestHandler<GetClassRoomsByDynamicQuery, PageableModel<GetClassRoomModel>>
{
    public async Task<PageableModel<GetClassRoomModel>> Handle(GetClassRoomsByDynamicQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.Dynamic ??= new Dynamic();

        var classRooms = await classRoomDal.GetPageListAsyncAutoMapperByDynamic<GetClassRoomModel>(
            dynamic: request.Dynamic,
            predicate: x => commonService.HttpUserType == UserTypes.Administator || x.School!.Id == commonService.HttpSchoolId,
            include: x => x.Include(u => u.School).Include(u => u.Package)
                           .Include(u => u.TeacherClassRooms).ThenInclude(u => u.Teacher)
                           .Include(u => u.Students),
            defaultOrderColumnName: x => x.CreateDate,
            enableTracking: false,
            configurationProvider: mapper.ConfigurationProvider,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
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

            result.Items = [.. result.Items.Where(x => teacher.ClassRoomId.Any(a => a == x.Id))];
        }

        return result;
    }
}