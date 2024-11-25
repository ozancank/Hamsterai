using Application.Features.Packages.Models.Packages;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Schools.Queries.Schools;

public class GetPackageSchoolsQuery : IRequest<List<GetPackageModel>>, ISecuredRequest<UserTypes>
{
    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
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
            predicate: x => x.IsActive && x.PackageUsers.Any(p => p.IsActive && p.User != null && p.User.SchoolId == schoolId && p.User.Type == UserTypes.School && p.IsActive),
            include: x => x.Include(x => x.PackageUsers).ThenInclude(u => u.User),
            orderBy: x => x.OrderBy(x => x.Name),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        return result;
    }
}