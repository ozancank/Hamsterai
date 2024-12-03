using Application.Features.Questions.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Questions.Commands.Questions;

public class UpdateQuestionStatusCommand : IRequest<bool>, ISecuredRequest<UserTypes>
{
    public Guid Id { get; set; }
    public QuestionStatus Status { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class UpdateQuestionStatusCommandHandler(IQuestionDal questionDal,
                                                ICommonService commonService) : IRequestHandler<UpdateQuestionStatusCommand, bool>
{
    public async Task<bool> Handle(UpdateQuestionStatusCommand request, CancellationToken cancellationToken)
    {
        var question = await questionDal.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        await QuestionRules.QuestionShouldExists(question);

        if (question.ManuelSendAgain) throw new BusinessException("Soru tekrar gönderilmiş, durumu değiştirilemez.");

        question.ManuelSendAgain = true;
        question.Status = request.Status;
        question.UpdateUser = commonService.HttpUserId;
        question.UpdateDate = DateTime.Now;

        await questionDal.UpdateAsync(question, cancellationToken: cancellationToken);
        return true;
    }
}