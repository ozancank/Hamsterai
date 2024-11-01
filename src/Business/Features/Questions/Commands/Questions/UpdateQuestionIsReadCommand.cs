using Business.Features.Lessons.Rules;
using Business.Features.Questions.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Questions.Commands.Questions;

public class UpdateQuestionIsReadCommand : IRequest<bool>, ISecuredRequest<UserTypes>
{
    public Guid Id { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class UpdateQuestionIsReadCommandHandler(IQuestionDal questionDal,
                                                ICommonService commonService) : IRequestHandler<UpdateQuestionIsReadCommand, bool>
{
    public async Task<bool> Handle(UpdateQuestionIsReadCommand request, CancellationToken cancellationToken)
    {
        var question = await questionDal.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        await QuestionRules.QuestionShouldExists(question);

        if (question.IsRead) return true;

        question.IsRead = true;
        question.UpdateUser = commonService.HttpUserId;
        question.UpdateDate = DateTime.Now;

        await questionDal.UpdateAsync(question, cancellationToken: cancellationToken);
        return true;
    }
}