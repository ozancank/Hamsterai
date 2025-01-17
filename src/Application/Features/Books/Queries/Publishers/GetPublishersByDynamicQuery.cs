using Application.Features.Books.Models.Publisher;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Books.Queries.Publishers;

public class GetPublishersByDynamicQuery : IRequest<PageableModel<GetPublisherModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required Dynamic Dynamic { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
}

public class GetPublishersByDynamicQueryHandler(IMapper mapper,
                                                IUserDal userDal,
                                                IPublisherDal publisherDal) : IRequestHandler<GetPublishersByDynamicQuery, PageableModel<GetPublisherModel>>
{
    public async Task<PageableModel<GetPublisherModel>> Handle(GetPublishersByDynamicQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.Dynamic ??= new Dynamic();

        var publishers = await publisherDal.GetPageListAsyncAutoMapperByDynamic<GetPublisherModel>(
            dynamic: request.Dynamic,
            defaultOrderColumnName: x => x.Name,
            enableTracking: false,
            include: x => x.Include(u => u.Books),
            configurationProvider: mapper.ConfigurationProvider,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);

        var list = mapper.Map<PageableModel<GetPublisherModel>>(publishers);
        return list;
    }
}