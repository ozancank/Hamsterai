using Business.Features.Schools.Models.Teachers;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Schools.Queries.Teachers;

public class GetTeachersByDynamicQuery : IRequest<PageableModel<GetTeacherModel>>, ISecuredRequest<UserTypes>
{
    public PageRequest PageRequest { get; set; }
    public Dynamic Dynamic { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
}

public class GetTeachersByDynamicQueryHandler(IMapper mapper,
                                              ICommonService commonService,
                                              ITeacherDal teacherDal) : IRequestHandler<GetTeachersByDynamicQuery, PageableModel<GetTeacherModel>>
{
    public async Task<PageableModel<GetTeacherModel>> Handle(GetTeachersByDynamicQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.Dynamic ??= new Dynamic();

        var teachers = await teacherDal.GetPageListAsyncAutoMapperByDynamic<GetTeacherModel>(
            dynamic: request.Dynamic,
            defaultOrderColumnName: x => x.CreateDate,
            enableTracking: false,
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            predicate: x => x.SchoolId == commonService.HttpSchoolId.Value,
            include: x => x.Include(u => u.School).Include(u => u.TeacherLessons).Include(u => u.TeacherLessons),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);
        var result = mapper.Map<PageableModel<GetTeacherModel>>(teachers);

        return result;
    }
}