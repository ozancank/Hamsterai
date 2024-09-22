using Business.Features.Schools.Models.Schools;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Schools.Queries.Schools;

public class GetSchoolsQuery : IRequest<PageableModel<GetSchoolModel>>, ISecuredRequest<UserTypes>
{
    public PageRequest PageRequest { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
}

public class GetSchoolsQueryHandler(IMapper mapper,
                                    ISchoolDal schoolDal) : IRequestHandler<GetSchoolsQuery, PageableModel<GetSchoolModel>>
{
    public async Task<PageableModel<GetSchoolModel>> Handle(GetSchoolsQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();

        var schools = await schoolDal.GetPageListAsyncAutoMapper<GetSchoolModel>(
            enableTracking: false,
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            include: x => x.Include(u => u.Users),
            orderBy: x => x.OrderBy(x => x.CreateDate),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);
        var result = mapper.Map<PageableModel<GetSchoolModel>>(schools);
        return result;
    }
}