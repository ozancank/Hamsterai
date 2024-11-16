using Application.Features.Packages.Models.Packages;
using Application.Features.Packages.Rules;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Packages.Queries.Packages;

public class GetPackageByIdQuery : IRequest<GetPackageModel>, ISecuredRequest<UserTypes>
{
    public short Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;
    public bool ForWeb { get; set; } = false;

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => true;
}

public class GetPackageByIdQueryHandler(IMapper mapper,
                                        IPackageDal packageDal) : IRequestHandler<GetPackageByIdQuery, GetPackageModel>
{
    public async Task<GetPackageModel> Handle(GetPackageByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await packageDal.GetAsyncAutoMapper<GetPackageModel>(
            enableTracking: request.Tracking,
            predicate: x => x.Id == request.Id && (!request.ForWeb || x.IsWebVisible),
            include: x => x.Include(u => u.RPackageLessons).ThenInclude(u => u.Lesson),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await PackageRules.PackageShouldExists(entity);

        if (request.ForWeb && entity.Category != null && !entity.Category.IsActive && !entity.Category.IsWebVisible)
            entity.Category = null;

        return entity;
    }
}