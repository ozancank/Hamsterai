using Business.Features.Students.Models;
using Business.Services.CommonService;
using Domain.Entities;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Students.Queries;

public class GetStudentsByDynamicQuery : IRequest<PageableModel<GetStudentModel>>, ISecuredRequest<UserTypes>
{
    public PageRequest PageRequest { get; set; }
    public Dynamic Dynamic { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School, UserTypes.Teacher];
}

public class GetStudentsByDynamicQueryHandler(IMapper mapper,
                                              ICommonService commonService,
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
            predicate: x => commonService.HttpUserType == UserTypes.Administator || x.ClassRoom.SchoolId == schoolId,
            include: x => x.Include(u => u.ClassRoom).Include(u => u.Teachers),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);
        var result = mapper.Map<PageableModel<GetStudentModel>>(students);

        return result;
    }
}