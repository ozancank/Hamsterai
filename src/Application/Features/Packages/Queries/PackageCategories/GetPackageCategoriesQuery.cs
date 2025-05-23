﻿using Application.Features.Packages.Models.PackageCategories;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Packages.Queries.PackageCategories;

public class GetPackageCategoriesQuery : IRequest<PageableModel<GetPackageCategoryModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public bool ForWeb { get; set; } = false;

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => true;
}

public class GetPackageCategoriesQueryHandler(IMapper mapper,
                                              IPackageCategoryDal packageCategoryDal) : IRequestHandler<GetPackageCategoriesQuery, PageableModel<GetPackageCategoryModel>>
{
    public async Task<PageableModel<GetPackageCategoryModel>> Handle(GetPackageCategoriesQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();

        var entities = await packageCategoryDal.GetPageListAsyncAutoMapper<GetPackageCategoryModel>(
            enableTracking: false,
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            predicate: x => !request.ForWeb || (x.IsWebVisible && x.IsActive),
            orderBy: x => x.OrderBy(x => x.SortNo).ThenBy(x => x.CreateDate),
            include: x => x.Include(u => u.Packages).ThenInclude(u => u.RPackageLessons).ThenInclude(u => u.Lesson),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);
        var list = mapper.Map<PageableModel<GetPackageCategoryModel>>(entities);

        foreach (var item in list.Items)
        {
            item.Packages = [.. item.Packages.Where(x => x.IsActive && (!request.ForWeb || x.IsWebVisible))];

            if (item.ParentId > 0) continue;
            {
                item.TopCategory = await packageCategoryDal.GetAsyncAutoMapper<GetPackageCategoryLiteModel>(
                    enableTracking: false,
                    predicate: x => x.Id == item.ParentId && (!request.ForWeb || (x.IsWebVisible && x.IsActive)),
                    configurationProvider: mapper.ConfigurationProvider,
                    cancellationToken: cancellationToken);
            }

            item.SubCategories = await packageCategoryDal.GetListAsyncAutoMapper<GetPackageCategoryLiteModel>(
                enableTracking: false,
                predicate: x => x.ParentId == item.Id && (!request.ForWeb || (x.IsWebVisible && x.IsActive)),
                configurationProvider: mapper.ConfigurationProvider,
                cancellationToken: cancellationToken);

            item.SubCategoryIds = [.. item.SubCategories.Where(x => !request.ForWeb || (x.IsWebVisible && x.IsActive)).Select(x => x.Id)];
        }

        return list;
    }
}