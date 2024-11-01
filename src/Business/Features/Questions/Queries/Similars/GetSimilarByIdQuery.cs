using Business.Features.Questions.Models.Similars;
using Business.Features.Questions.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Questions.Queries.Similars;

public class GetSimilarByIdQuery : IRequest<GetSimilarModel>, ISecuredRequest<UserTypes>
{
    public Guid Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetSimilarByIdQueryHandler(IMapper mapper,
                                        ISimilarDal similarDal,
                                        ICommonService commonService) : IRequestHandler<GetSimilarByIdQuery, GetSimilarModel>
{
    public async Task<GetSimilarModel> Handle(GetSimilarByIdQuery request, CancellationToken cancellationToken)
    {
        var similar = await similarDal.GetAsyncAutoMapper<GetSimilarModel>(
            predicate: x => x.Id == request.Id && x.CreateUser == commonService.HttpUserId && x.IsActive,
            enableTracking: request.Tracking,
            include: x => x.Include(u => u.Lesson),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await SimilarRules.SimilarQuestionShouldExists(similar);
        return similar;
    }
}