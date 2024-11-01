using Business.Features.Questions.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Questions.Commands.Similars;

public class ActiveSimilarCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public Guid QuestionId { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class ActiveSimilarCommandHandler(ISimilarDal similarQuestionDal,
                                         ICommonService commonService) : IRequestHandler<ActiveSimilarCommand, bool>
{
    public async Task<bool> Handle(ActiveSimilarCommand request, CancellationToken cancellationToken)
    {
        var question = await similarQuestionDal.GetAsync(predicate: x => x.Id == request.QuestionId, cancellationToken: cancellationToken);
        await SimilarRules.SimilarQuestionShouldExists(question);

        question.IsActive = true;
        question.UpdateUser = commonService.HttpUserId;
        question.UpdateDate = DateTime.Now;

        await similarQuestionDal.UpdateAsync(question, cancellationToken: cancellationToken);

        return true;
    }
}