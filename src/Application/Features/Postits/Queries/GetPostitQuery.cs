using Application.Features.Postits.Models;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Postits.Queries;

public class GetPostitsQuery : IRequest<PageableModel<GetPostitModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required PostitRequestModel Model { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetPostitsQueryHandler(IMapper mapper,
                                    IPostitDal postitDal,
                                    ICommonService commonService) : IRequestHandler<GetPostitsQuery, PageableModel<GetPostitModel>>
{
    public async Task<PageableModel<GetPostitModel>> Handle(GetPostitsQuery request, CancellationToken cancellationToken)
    {
        var userId = commonService.HttpUserId;

        request.PageRequest ??= new PageRequest();
        request.Model ??= new PostitRequestModel();

        var postits = await postitDal.GetPageListAsyncAutoMapper<GetPostitModel>(
            enableTracking: false,
            predicate: x => x.CreateUser == userId
                            && (request.Model.LessonId <= 0 || x.LessonId == request.Model.LessonId)
                            && (request.Model.StartDate == null || x.CreateDate >= request.Model.StartDate)
                            && (request.Model.EndDate == null || x.CreateDate <= request.Model.EndDate),
            include: x => x.Include(u => u.Lesson),
            orderBy: x => x.OrderBy(o => o.SortNo).ThenByDescending(o => o.CreateDate),
            configurationProvider: mapper.ConfigurationProvider,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetPostitModel>>(postits);
        return result;
    }
}