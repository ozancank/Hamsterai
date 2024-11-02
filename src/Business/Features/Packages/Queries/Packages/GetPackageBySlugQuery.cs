using Business.Features.Packages.Models.Packages;
using Business.Features.Packages.Rules;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Packages.Queries.Packages;

public class GetPackageBySlugQuery : IRequest<GetPackageModel>, ISecuredRequest<UserTypes>
{
    public required string Slug { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => true;
}

public class GetPackageBySlugQueryHandler(IMapper mapper,
                                          IPackageDal packageDal) : IRequestHandler<GetPackageBySlugQuery, GetPackageModel>
{
    public async Task<GetPackageModel> Handle(GetPackageBySlugQuery request, CancellationToken cancellationToken)
    {
        var entity = await packageDal.GetAsyncAutoMapper<GetPackageModel>(
            enableTracking: request.Tracking,
            predicate: x => x.Slug == request.Slug,
            include: x => x.Include(u => u.RPackageLessons).ThenInclude(u => u.Lesson),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await PackageRules.PackageShouldExists(entity);
        return entity;
    }
}