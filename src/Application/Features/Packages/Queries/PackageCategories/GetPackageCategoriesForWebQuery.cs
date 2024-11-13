using Application.Features.Packages.Models.PackageCategories;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Packages.Queries.PackageCategories;

public class GetPackageCategoriesForWebQuery : IRequest<PageableModel<GetPackageCategoryModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => true;
}

public class GetPackageCategoriesForWebQueryHandler(IMapper mapper,
                                                    IPackageCategoryDal packageCategoryDal) : IRequestHandler<GetPackageCategoriesForWebQuery, PageableModel<GetPackageCategoryModel>>
{
    public async Task<PageableModel<GetPackageCategoryModel>> Handle(GetPackageCategoriesForWebQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();

        var entities = await packageCategoryDal.GetPageListAsyncAutoMapper<GetPackageCategoryModel>(
            enableTracking: false,
            predicate: x => x.IsActive && x.IsWebVisible,
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            orderBy: x => x.OrderBy(x => x.SortNo).ThenBy(x => x.CreateDate),
            include: x => x.Include(u => u.Packages.Where(p => p.IsWebVisible && p.IsActive))
                           .ThenInclude(u => u.RPackageLessons.Where(p => p.IsActive && p.Lesson != null && p.Lesson.IsActive))
                           .ThenInclude(u => u.Lesson),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);
        var list = mapper.Map<PageableModel<GetPackageCategoryModel>>(entities);

        foreach (var item in list.Items)
        {
            if (item.ParentId > 0)
            {
                item.TopCategory = await packageCategoryDal.GetAsyncAutoMapper<GetPackageCategoryLiteModel>(
                    enableTracking: false,
                    predicate: x => x.Id == item.ParentId && x.IsWebVisible && x.IsActive,
                    configurationProvider: mapper.ConfigurationProvider,
                    cancellationToken: cancellationToken);
            }

            item.SubCategories = await packageCategoryDal.GetListAsyncAutoMapper<GetPackageCategoryLiteModel>(
                enableTracking: false,
                predicate: x => x.ParentId == item.Id && x.IsWebVisible && x.IsActive,
                configurationProvider: mapper.ConfigurationProvider,
                cancellationToken: cancellationToken);

            item.SubCategoryIds = item.SubCategories.Select(x => x.Id).ToList();
        }

        return list;
    }
}