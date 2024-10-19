using Business.Features.Students.Models;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Students.Queries;

public class GetStudentsQuery : IRequest<PageableModel<GetStudentModel>>, ISecuredRequest<UserTypes>
{
    public PageRequest PageRequest { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School, UserTypes.Teacher];
}

public class GetStudentsQueryHandler(IMapper mapper,
                                     ICommonService commonService,
                                     IStudentDal studentDal) : IRequestHandler<GetStudentsQuery, PageableModel<GetStudentModel>>
{
    public async Task<PageableModel<GetStudentModel>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();

        var students = await studentDal.GetPageListAsyncAutoMapper<GetStudentModel>(
            enableTracking: false,
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            predicate: x => commonService.HttpUserType == UserTypes.Administator || x.ClassRoom.SchoolId == commonService.HttpSchoolId,
            include: x => x.Include(u => u.ClassRoom).Include(u => u.Teachers),
            orderBy: x => x.OrderBy(x => x.CreateDate),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);
        var result = mapper.Map<PageableModel<GetStudentModel>>(students);
        return result;
    }
}