using Application.Features.Packages.Models.PackageCategories;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Packages.Queries.PackageCategories;

public class GetPackageCategoriesByDynamicQuery : IRequest<PageableModel<GetPackageCategoryModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required Dynamic Dynamic { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetPackageCategoriesByDynamicQueryHandler(IMapper mapper,
                                                       IPackageCategoryDal packageCategoryDal) : IRequestHandler<GetPackageCategoriesByDynamicQuery, PageableModel<GetPackageCategoryModel>>
{
    public async Task<PageableModel<GetPackageCategoryModel>> Handle(GetPackageCategoriesByDynamicQuery request, CancellationToken cancellationToken)
    {
        var users = await packageCategoryDal.GetPageListAsyncAutoMapperByDynamic<GetPackageCategoryModel>(
            dynamic: request.Dynamic,
            defaultOrderColumnName: x => x.SortNo,
            enableTracking: false,
            include: x => x.Include(u => u.Packages).ThenInclude(u => u.RPackageLessons).ThenInclude(u => u.Lesson),
            configurationProvider: mapper.ConfigurationProvider,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);
        var list = mapper.Map<PageableModel<GetPackageCategoryModel>>(users);

        foreach (var item in list.Items)
        {
            if (item.ParentId > 0)
            {
                item.TopCategory = await packageCategoryDal.GetAsyncAutoMapper<GetPackageCategoryLiteModel>(
                    enableTracking: false,
                    predicate: x => x.Id == item.ParentId,
                    configurationProvider: mapper.ConfigurationProvider,
                    cancellationToken: cancellationToken);
            }

            item.SubCategories = await packageCategoryDal.GetListAsyncAutoMapper<GetPackageCategoryLiteModel>(
                enableTracking: false,
                predicate: x => x.ParentId == item.Id,
                configurationProvider: mapper.ConfigurationProvider,
                cancellationToken: cancellationToken);

            item.SubCategoryIds = [.. item.SubCategories.Select(x => x.Id)];
        }

        return list;
    }
}