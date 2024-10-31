using Business.Features.Packages.Models;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Packages.Queries;

public class GetPackagesQuery : IRequest<PageableModel<GetPackageModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }

    public UserTypes[] Roles { get; } = [];
}

public class GetPackagesQueryHandler(IMapper mapper,
                                     IPackageDal packageDal) : IRequestHandler<GetPackagesQuery, PageableModel<GetPackageModel>>
{
    public async Task<PageableModel<GetPackageModel>> Handle(GetPackagesQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();

        var entities = await packageDal.GetPageListAsyncAutoMapper<GetPackageModel>(
            enableTracking: false,
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