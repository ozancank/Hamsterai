using Business.Features.Packages.Models;
using Business.Features.Packages.Rules;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Packages.Queries;

public class GetPackageByIdQuery : IRequest<GetPackageModel>, ISecuredRequest<UserTypes>
{
    public byte Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetPackageByIdQueryHandler(IMapper mapper,
                                        IPackageDal packageDal) : IRequestHandler<GetPackageByIdQuery, GetPackageModel>
{
    public async Task<GetPackageModel> Handle(GetPackageByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await packageDal.GetAsyncAutoMapper<GetPackageModel>(
            enableTracking: request.Tracking,
            predicate: x => x.Id == request.Id,
            include: x => x.Include(u => u.RPackageLessons).ThenInclude(u => u.Lesson),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await PackageRules.PackageShouldExists(entity);
        return entity;
    }
}