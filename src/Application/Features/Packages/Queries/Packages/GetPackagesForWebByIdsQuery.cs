using Application.Features.Packages.Models.Packages;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Packages.Queries.Packages;

public class GetPackagesForWebByIdsQuery : IRequest<List<GetPackageModel>>, ISecuredRequest<UserTypes>
{
    public required List<short> PackageIds { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => true;
}

public class GetPackagesForWebByIdsQueryHandler(IMapper mapper,
                                                IPackageDal packageDal) : IRequestHandler<GetPackagesForWebByIdsQuery, List<GetPackageModel>>
{
    public async Task<List<GetPackageModel>> Handle(GetPackagesForWebByIdsQuery request, CancellationToken cancellationToken)
    {
        var entities = await packageDal.GetListAsyncAutoMapper<GetPackageModel>(
            enableTracking: false,
            predicate: x => x.IsActive && x.IsWebVisible && request.PackageIds.Contains(x.Id),
            orderBy: x => x.OrderBy(x => x.SortNo).ThenBy(x => x.CreateDate),
            include: x => x.Include(u => u.RPackageLessons).ThenInclude(u => u.Lesson),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);
        return entities;
    }
}