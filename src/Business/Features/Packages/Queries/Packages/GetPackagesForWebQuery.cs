using Business.Features.Packages.Models.Packages;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Packages.Queries.Packages;

public class GetPackagesForWebQuery : IRequest<PageableModel<GetPackageModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => true;
}

public class GetPackagesForWebQueryHandler(IMapper mapper,
                                           IPackageDal packageDal) : IRequestHandler<GetPackagesForWebQuery, PageableModel<GetPackageModel>>
{
    public async Task<PageableModel<GetPackageModel>> Handle(GetPackagesForWebQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();

        var entities = await packageDal.GetPageListAsyncAutoMapper<GetPackageModel>(
            enableTracking: false,
            predicate: x => x.IsActive && x.IsWebVisible,
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            orderBy: x => x.OrderBy(x => x.SortNo).ThenBy(x => x.CreateDate),
            include: x => x.Include(u => u.RPackageLessons).ThenInclude(u => u.Lesson),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);
        var result = mapper.Map<PageableModel<GetPackageModel>>(entities);
        return result;
    }
}