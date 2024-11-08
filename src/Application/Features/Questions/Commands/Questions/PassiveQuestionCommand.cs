using Application.Features.Questions.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Questions.Commands.Questions;

public class PassiveQuestionCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public Guid QuestionId { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class PassiveQuestionCommandHandler(IQuestionDal questionDal,
                                           ICommonService commonService) : IRequestHandler<PassiveQuestionCommand, bool>
{
    public async Task<bool> Handle(PassiveQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await questionDal.GetAsync(predicate: x => x.Id == request.QuestionId, cancellationToken: cancellationToken);
        await QuestionRules.QuestionShouldExists(question);

        question.IsActive = false;
        question.UpdateUser = commonService.HttpUserId;
        question.UpdateDate = DateTime.Now;

        await questionDal.UpdateAsync(question, cancellationToken: cancellationToken);

        return true;
    }
}