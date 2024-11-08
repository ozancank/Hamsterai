using Application.Features.Teachers.Models;
using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Teachers.Queries;

public class GetTeachersQuery : IRequest<PageableModel<GetTeacherModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School];
    public bool AllowByPass => false;
}

public class GetTeachersQueryHandler(IMapper mapper,
                                     ICommonService commonService,
                                     IUserDal userDal,
                                     ITeacherDal teacherDal) : IRequestHandler<GetTeachersQuery, PageableModel<GetTeacherModel>>
{
    public async Task<PageableModel<GetTeacherModel>> Handle(GetTeachersQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        var schoolId = commonService.HttpSchoolId ?? 0;

        var teachers = await teacherDal.GetPageListAsyncAutoMapper<GetTeacherModel>(
            enableTracking: false,
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            predicate: x => commonService.HttpUserType == UserTypes.Administator || x.SchoolId == schoolId,
            include: x => x.Include(u => u.School).Include(u => u.RTeacherLessons).Include(u => u.RTeacherLessons),
            orderBy: x => x.OrderBy(x => x.CreateDate),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        await teachers.Items.ForEachAsync(async x =>
        {
            x.UserId = (await userDal.GetAsync(u => u.Type == UserTypes.Teacher && u.ConnectionId == x.Id, enableTracking: false, cancellationToken: cancellationToken))?.Id ?? 0;
        });

        var result = mapper.Map<PageableModel<GetTeacherModel>>(teachers);
        return result;
    }
}