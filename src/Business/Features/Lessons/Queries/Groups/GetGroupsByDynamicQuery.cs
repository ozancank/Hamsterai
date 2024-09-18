using Business.Features.Lessons.Models.Groups;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Lessons.Queries.Groups;

public class GetGroupsByDynamicQuery : IRequest<PageableModel<GetGroupModel>>, ISecuredRequest<UserTypes>
{
    public PageRequest PageRequest { get; set; }
    public Dynamic Dynamic { get; set; }

    public UserTypes[] Roles { get; } = [];
}

public class GetGroupsByDynamicQueryHandler(IMapper mapper,
                                            IGroupDal groupDal) : IRequestHandler<GetGroupsByDynamicQuery, PageableModel<GetGroupModel>>
{
    public async Task<PageableModel<GetGroupModel>> Handle(GetGroupsByDynamicQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.Dynamic ??= new Dynamic();

        var users = await groupDal.GetPageListAsyncAutoMapperByDynamic<GetGroupModel>(
            dynamic: request.Dynamic,
            defaultOrderColumnName: x => x.CreateDate,
            enableTracking: false,
            configurationProvider: mapper.ConfigurationProvider,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);
        var list = mapper.Map<PageableModel<GetGroupModel>>(users);
        return list;
    }
}