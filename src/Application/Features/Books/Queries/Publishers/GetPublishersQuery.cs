using Application.Features.Books.Models.Publisher;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Books.Queries.Publishers;

public class GetPublishersQuery : IRequest<PageableModel<GetPublisherModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School];
    public bool AllowByPass => false;
}

public class GetPublishersQueryHandler(IMapper mapper,
                                       IPublisherDal pPublisherDal) : IRequestHandler<GetPublishersQuery, PageableModel<GetPublisherModel>>
{
    public async Task<PageableModel<GetPublisherModel>> Handle(GetPublishersQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();

        var pPublishers = await pPublisherDal.GetPageListAsyncAutoMapper<GetPublisherModel>(
            enableTracking: false,
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            include: x => x.Include(u => u.Books),
            orderBy: x => x.OrderBy(x => x.Name),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetPublisherModel>>(pPublishers);
        return result;
    }
}