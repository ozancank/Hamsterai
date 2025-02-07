using Application.Features.Questions.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Questions.Commands.Questions;

public class UpdateQuestionManuelSendCommand : IRequest<bool>, ISecuredRequest<UserTypes>
{
    public Guid Id { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class UpdateQuestionManuelSendCommandHandler(IQuestionDal questionDal,
                                                    ICommonService commonService) : IRequestHandler<UpdateQuestionManuelSendCommand, bool>
{
    public async Task<bool> Handle(UpdateQuestionManuelSendCommand request, CancellationToken cancellationToken)
    {
        var userId = commonService.HttpUserId;
        var question = await questionDal.GetAsync(x => x.Id == request.Id && x.CreateUser == userId, cancellationToken: cancellationToken);

        await QuestionRules.QuestionShouldExists(question);

        if (question.ManuelSendAgain) throw new BusinessException("Soru tekrar gönderilmiş, durumu değiştirilemez.");

        question.ManuelSendAgain = true;
        question.Status = QuestionStatus.Waiting;
        question.TryCount = 0;
        question.UpdateUser = userId;
        question.UpdateDate = DateTime.Now;

        await questionDal.UpdateAsync(question, cancellationToken: cancellationToken);
        return true;
    }
}