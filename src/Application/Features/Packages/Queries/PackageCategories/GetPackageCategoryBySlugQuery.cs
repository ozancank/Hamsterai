using Application.Features.Packages.Models.PackageCategories;
using Application.Features.Packages.Rules;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Packages.Queries.PackageCategories;

public class GetPackageCategoryBySlugQuery : IRequest<GetPackageCategoryModel>, ISecuredRequest<UserTypes>
{
    public required string Slug { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;
    public bool ForWeb { get; set; } = true;

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => true;
}

public class GetPackageCategoryBySlugQueryHandler(IMapper mapper,
                                                  IPackageCategoryDal packageCategoryDal) : IRequestHandler<GetPackageCategoryBySlugQuery, GetPackageCategoryModel>
{
    public async Task<GetPackageCategoryModel> Handle(GetPackageCategoryBySlugQuery request, CancellationToken cancellationToken)
    {
        var entity = await packageCategoryDal.GetAsyncAutoMapper<GetPackageCategoryModel>(
            enableTracking: request.Tracking,
            predicate: x => x.Slug == request.Slug && (!request.ForWeb || x.IsWebVisible),
            include: x => x.Include(u => u.Packages).ThenInclude(u => u.RPackageLessons).ThenInclude(u => u.Lesson),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await PackageRules.PackageShouldExists(entity);

        if (request.ForWeb)
            entity.Packages = entity.Packages.Where(x => x.IsActive && x.IsWebVisible).ToList();

        if (entity.ParentId > 0)
        {
            entity.TopCategory = await packageCategoryDal.GetAsyncAutoMapper<GetPackageCategoryLiteModel>(
                    enableTracking: false,
                    predicate: x => x.Id == entity.ParentId && (!request.ForWeb || (x.IsWebVisible && x.IsActive)),
                    configurationProvider: mapper.ConfigurationProvider,
                    cancellationToken: cancellationToken);
        }

        entity.SubCategories = await packageCategoryDal.GetListAsyncAutoMapper<GetPackageCategoryLiteModel>(
                enableTracking: false,
                predicate: x => x.ParentId == entity.Id && (!request.ForWeb || (x.IsWebVisible && x.IsActive)),
                configurationProvider: mapper.ConfigurationProvider,
                cancellationToken: cancellationToken);

        entity.SubCategoryIds = entity.SubCategories.Where(x => request.ForWeb || x.IsWebVisible).Select(x => x.Id).ToList();

        return entity;
    }
}