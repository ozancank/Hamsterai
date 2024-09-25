using Business.Features.Questions.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Questions.Commands.Similars;

public class UpdateSimilarIsReadCommand : IRequest<bool>, ISecuredRequest<UserTypes>
{
    public Guid Id { get; set; }

    public UserTypes[] Roles { get; } = [];
}

public class UpdateSimilarIsReadCommandHandler(ISimilarQuestionDal similarDal,
                                               ICommonService commonService) : IRequestHandler<UpdateSimilarIsReadCommand, bool>
{
    public async Task<bool> Handle(UpdateSimilarIsReadCommand request, CancellationToken cancellationToken)
    {
        var similar = await similarDal.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        await SimilarRules.SimilarQuestionShouldExists(similar);

        if (similar.IsRead) return true;

        similar.IsRead = true;
        similar.UpdateUser = commonService.HttpUserId;
        similar.UpdateDate = DateTime.Now;

        await similarDal.UpdateAsync(similar);
        return true;
    }
}