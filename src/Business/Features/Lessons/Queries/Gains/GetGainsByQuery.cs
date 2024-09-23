using Business.Features.Lessons.Models.Gains;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Lessons.Queries.Gains;

public class GetGainsQuery : IRequest<PageableModel<GetGainModel>>, ISecuredRequest<UserTypes>
{
    public PageRequest PageRequest { get; set; }

    public UserTypes[] Roles { get; } = [];
}

public class GetGainsQueryQueryHandler(IMapper mapper,
                                       IGainDal gainDal) : IRequestHandler<GetGainsQuery, PageableModel<GetGainModel>>
{
    public async Task<PageableModel<GetGainModel>> Handle(GetGainsQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();

        var gains = await gainDal.GetPageListAsyncAutoMapper<GetGainModel>(
            enableTracking: false,
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            orderBy: x => x.OrderBy(x => x.CreateDate),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);
        var result = mapper.Map<PageableModel<GetGainModel>>(gains);
        return result;
    }
}