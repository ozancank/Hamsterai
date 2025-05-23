﻿using Application.Features.Packages.Models.Packages;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Packages.Queries.Packages;

public class GetPackagesByDynamicQuery : IRequest<PageableModel<GetPackageModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required Dynamic Dynamic { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetPackagesByDynamicQueryHandler(IMapper mapper,
                                              IPackageDal packageDal) : IRequestHandler<GetPackagesByDynamicQuery, PageableModel<GetPackageModel>>
{
    public async Task<PageableModel<GetPackageModel>> Handle(GetPackagesByDynamicQuery request, CancellationToken cancellationToken)
    {
        var users = await packageDal.GetPageListAsyncAutoMapperByDynamic<GetPackageModel>(
            dynamic: request.Dynamic,
            defaultOrderColumnName: x => x.SortNo,
            enableTracking: false,
            include: x => x.Include(u => u.RPackageLessons).ThenInclude(u => u.Lesson),
            configurationProvider: mapper.ConfigurationProvider,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);
        var list = mapper.Map<PageableModel<GetPackageModel>>(users);
        return list;
    }
}