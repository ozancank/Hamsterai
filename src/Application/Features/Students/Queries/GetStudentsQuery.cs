using Application.Features.Students.Models;
using Application.Services.CommonService;
using DataAccess.Abstract.Core;
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

        var students = await studentDal.GetPageListAsyncAutoMapper<GetStudentModel>(
            enableTracking: false,
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            predicate: x => commonService.HttpUserType == UserTypes.Administator || x.ClassRoom!.SchoolId == commonService.HttpSchoolId,
            include: x => x.Include(u => u.ClassRoom).Include(u => u.Teachers),
            orderBy: x => x.OrderBy(x => x.CreateDate),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        var users = await userDal.GetListAsync(
            predicate: x => x.Type == UserTypes.Student && students.Items.Any(s => s.Id == x.ConnectionId),
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
        return result;
    }
}