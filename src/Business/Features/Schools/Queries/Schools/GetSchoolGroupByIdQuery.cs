using Business.Features.Packages.Models;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Schools.Queries.Schools;

public class GetPackageSchoolsQuery : IRequest<List<GetPackageModel>>, ISecuredRequest<UserTypes>
{
    public UserTypes[] Roles { get; } = [];
}

public class GetPackageSchoolsQueryHandler(IMapper mapper,
                                           ICommonService commonService,
                                           IPackageDal packageDal) : IRequestHandler<GetPackageSchoolsQuery, List<GetPackageModel>>
{
    public async Task<List<GetPackageModel>> Handle(GetPackageSchoolsQuery request, CancellationToken cancellationToken)
    {
        var schoolId = commonService.HttpSchoolId;

        var result = await packageDal.GetListAsyncAutoMapper<GetPackageModel>(
            enableTracking: false,
            predicate: x => x.IsActive && x.RPackageSchools.Any(s => s.SchoolId == schoolId && s.IsActive),
            include: x => x.Include(x => x.RPackageSchools),
            orderBy: x => x.OrderBy(x => x.Name),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        return result;
    }
}