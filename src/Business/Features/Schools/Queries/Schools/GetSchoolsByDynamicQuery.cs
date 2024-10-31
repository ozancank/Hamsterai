using Business.Features.Schools.Models.Schools;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Schools.Queries.Schools;

public class GetSchoolsByDynamicQuery : IRequest<PageableModel<GetSchoolModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required Dynamic Dynamic { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
}

public class GetSchoolsByDynamicQueryHandler(IMapper mapper,
                                             ISchoolDal schoolDal) : IRequestHandler<GetSchoolsByDynamicQuery, PageableModel<GetSchoolModel>>
{
    public async Task<PageableModel<GetSchoolModel>> Handle(GetSchoolsByDynamicQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.Dynamic ??= new Dynamic();

        var schools = await schoolDal.GetPageListAsyncAutoMapperByDynamic<GetSchoolModel>(
            dynamic: request.Dynamic,
            defaultOrderColumnName: x => x.CreateDate,
            enableTracking: false,
            include: x => x.Include(u => u.Users)
                           .Include(x => x.RPackageSchools).ThenInclude(u => u.Package),
            configurationProvider: mapper.ConfigurationProvider,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);
        var list = mapper.Map<PageableModel<GetSchoolModel>>(schools);
        return list;
    }
}