using Business.Features.Lessons.Models.Gains;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Lessons.Queries.Gains;

public class GetGainsQuery : IRequest<PageableModel<GetGainModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }

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
            orderBy: x => x.OrderBy(x => x.CreateDate),
            configurationProvider: mapper.ConfigurationProvider,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);
        var result = mapper.Map<PageableModel<GetGainModel>>(gains);
        return result;
    }
}