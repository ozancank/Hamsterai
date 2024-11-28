using Application.Features.Students.Models;
using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using Domain.Entities;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Students.Queries;

public class GetStudentsByDynamicQuery : IRequest<PageableModel<GetStudentModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required Dynamic Dynamic { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School, UserTypes.Teacher];
    public bool AllowByPass => false;
}

public class GetStudentsByDynamicQueryHandler(IMapper mapper,
                                              ICommonService commonService,
                                              IUserDal userDal,
                                              IStudentDal studentDal) : IRequestHandler<GetStudentsByDynamicQuery, PageableModel<GetStudentModel>>
{
    public async Task<PageableModel<GetStudentModel>> Handle(GetStudentsByDynamicQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.Dynamic ??= new Dynamic();
        var schoolId = commonService.HttpSchoolId ?? 0;

        var students = await studentDal.GetPageListAsyncAutoMapperByDynamic<GetStudentModel>(
            dynamic: request.Dynamic,
            defaultOrderColumnName: x => x.CreateDate,
            enableTracking: false,
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            predicate: x => commonService.HttpUserType == UserTypes.Administator || x.ClassRoom!.SchoolId == schoolId,
            include: x => x.Include(u => u.ClassRoom).Include(u => u.Teachers),
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
        });

        var result = mapper.Map<PageableModel<GetStudentModel>>(students);

        return result;
    }
}