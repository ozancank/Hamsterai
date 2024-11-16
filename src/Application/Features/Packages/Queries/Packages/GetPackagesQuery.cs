using Application.Features.Packages.Models.Packages;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Packages.Queries.Packages;

public class GetPackagesQuery : IRequest<PageableModel<GetPackageModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public bool ForWeb { get; set; } = false;

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => true;
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
            predicate: x => !request.ForWeb || (x.IsActive && x.IsWebVisible),
            orderBy: x => x.OrderBy(x => x.SortNo).ThenBy(x => x.CreateDate),
            include: x => x.Include(u => u.RPackageLessons).ThenInclude(u => u.Lesson),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);
        var result = mapper.Map<PageableModel<GetPackageModel>>(entities);

        result.Items.ForEach(x =>
        {
            if (request.ForWeb && x.Category != null && !x.Category.IsActive && !x.Category.IsWebVisible)
                x.Category = null;
        });

        return result;
    }
}