using Application.Features.Students.Models;
using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Utilities.Numbers;

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

        await students.Items.ForEachAsync(async x =>
        {
            x.UserId = (await userDal.GetAsync(u => u.Type == UserTypes.Student && u.ConnectionId == x.Id, enableTracking: false, cancellationToken: cancellationToken))?.Id ?? 0;
        });

        var result = mapper.Map<PageableModel<GetStudentModel>>(students);
        return result;
    }
}