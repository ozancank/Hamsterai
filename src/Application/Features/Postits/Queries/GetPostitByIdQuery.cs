using Application.Features.Postits.Models;
using Application.Features.Postits.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Postits.Queries;

public sealed class GetPostitByIdQuery : IRequest<GetPostitModel>, ISecuredRequest<UserTypes>
{
    public Guid Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public sealed class GetPostitByIdQueryHandler(IMapper mapper,
                                              ICommonService commonService,
                                              IPostitDal postitDal) : IRequestHandler<GetPostitByIdQuery, GetPostitModel>
{
    public async Task<GetPostitModel> Handle(GetPostitByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = commonService.HttpUserId;

        var postit = await postitDal.GetAsyncAutoMapper<GetPostitModel>(
            enableTracking: false,
            predicate: x => x.CreateUser == userId && x.Id == request.Id,
            include: x => x.Include(x => x.Lesson),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await PostitRules.PostitShouldExists(postit);

        return postit;
    }
}