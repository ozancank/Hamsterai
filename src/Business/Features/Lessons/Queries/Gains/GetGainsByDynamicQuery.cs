using Business.Features.Lessons.Models.Gains;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Lessons.Queries.Gains;

public class GetGainsByDynamicQuery : IRequest<PageableModel<GetGainModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required Dynamic Dynamic { get; set; }

    public UserTypes[] Roles { get; } = [];
}

public class GetGainsByDynamicQueryHandler(IMapper mapper,
                                           IGainDal gainDal) : IRequestHandler<GetGainsByDynamicQuery, PageableModel<GetGainModel>>
{
    public async Task<PageableModel<GetGainModel>> Handle(GetGainsByDynamicQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.Dynamic ??= new Dynamic();

        var gains = await gainDal.GetPageListAsyncAutoMapperByDynamic<GetGainModel>(
            dynamic: request.Dynamic,
            defaultOrderColumnName: x => x.CreateDate,
            enableTracking: false,
            configurationProvider: mapper.ConfigurationProvider,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);
        var list = mapper.Map<PageableModel<GetGainModel>>(gains);
        return list;
    }
}