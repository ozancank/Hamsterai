using Application.Features.Packages.Models.PackageCategories;
using Application.Features.Packages.Rules;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Packages.Queries.PackageCategories;

public class GetPackageCategoryByIdQuery : IRequest<GetPackageCategoryModel>, ISecuredRequest<UserTypes>
{
    public byte Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;
    public bool ForWeb { get; set; } = false;

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => true;
}

public class GetPackageCategoryByIdQueryHandler(IMapper mapper,
                                                IPackageCategoryDal packageCategoryDal) : IRequestHandler<GetPackageCategoryByIdQuery, GetPackageCategoryModel>
{
    public async Task<GetPackageCategoryModel> Handle(GetPackageCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await packageCategoryDal.GetAsyncAutoMapper<GetPackageCategoryModel>(
            enableTracking: request.Tracking,
            predicate: x => x.Id == request.Id && (!request.ForWeb || (x.IsWebVisible && x.IsActive)),
            include: x => x.Include(u => u.Packages).ThenInclude(u => u.RPackageLessons.Where(x => x.IsActive)).ThenInclude(u => u.Lesson),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await PackageRules.PackageShouldExists(entity);

        if (request.ForWeb)
            entity.Packages = [.. entity.Packages.Where(x => x.IsActive && x.IsWebVisible)];

        if (entity.ParentId > 0)
        {
            entity.TopCategory = await packageCategoryDal.GetAsyncAutoMapper<GetPackageCategoryLiteModel>(
                    enableTracking: false,
                    predicate: x => x.Id == entity.ParentId && x.IsActive && !request.ForWeb || (x.IsWebVisible && x.IsActive),
                    configurationProvider: mapper.ConfigurationProvider,
                    cancellationToken: cancellationToken);
        }

        entity.SubCategories = await packageCategoryDal.GetListAsyncAutoMapper<GetPackageCategoryLiteModel>(
                enableTracking: false,
                predicate: x => x.ParentId == entity.Id && x.IsActive && !request.ForWeb || (x.IsWebVisible && x.IsActive),
                configurationProvider: mapper.ConfigurationProvider,
                cancellationToken: cancellationToken);

        entity.SubCategoryIds = [.. entity.SubCategories.Where(x => !request.ForWeb || (x.IsWebVisible && x.IsActive)).Select(x => x.Id)];

        return entity;
    }
}