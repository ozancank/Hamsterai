using Application.Features.Questions.Models.Questions;
using Application.Features.Questions.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Questions.Commands.Questions;

public class UpdateQuestionTextCommand : IRequest<bool>, ISecuredRequest<UserTypes>
{
    public required UpdateQuestionTextRequestModel Model { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class UpdateQuestionTextCommandHandler(IQuestionDal questionDal,
                                              ICommonService commonService) : IRequestHandler<UpdateQuestionTextCommand, bool>
{
    public async Task<bool> Handle(UpdateQuestionTextCommand request, CancellationToken cancellationToken)
    {
        var userId = commonService.HttpUserId;
        var question = await questionDal.GetAsync(x => x.Id == request.Model.QuestionId && x.CreateUser == userId, cancellationToken: cancellationToken);

        await QuestionRules.QuestionShouldExists(question);

        question.QuestionText = request.Model.QuestionText;
        question.Status = QuestionStatus.ControlledForOcr;
        question.TryCount = 0;
        question.UpdateUser = userId;
        question.UpdateDate = DateTime.Now;

        await questionDal.UpdateAsync(question, cancellationToken: cancellationToken);
        return true;
    }
}