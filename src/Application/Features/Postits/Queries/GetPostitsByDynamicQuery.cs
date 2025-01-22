using Application.Features.Postits.Models;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Postits.Queries;

public class GetPostitsByDynamicQuery : IRequest<PageableModel<GetPostitModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required Dynamic Dynamic { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetPostitsByDynamicQueryHandler(IMapper mapper,
                                             IPostitDal postitDal,
                                             ICommonService commonService) : IRequestHandler<GetPostitsByDynamicQuery, PageableModel<GetPostitModel>>
{
    public async Task<PageableModel<GetPostitModel>> Handle(GetPostitsByDynamicQuery request, CancellationToken cancellationToken)
    {
        var userId = commonService.HttpUserId;

        request.PageRequest ??= new PageRequest();
        request.Dynamic ??= new Dynamic();

        var postits = await postitDal.GetPageListAsyncAutoMapperByDynamic<GetPostitModel>(
        dynamic: request.Dynamic,
            predicate: x => x.CreateUser == userId,
            enableTracking: false,
            include: x => x.Include(u => u.Lesson),
            defaultOrderColumnName: x => x.SortNo,
            configurationProvider: mapper.ConfigurationProvider,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetPostitModel>>(postits);
        return result;
    }
}