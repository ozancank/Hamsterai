using Business.Features.Questions.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Questions.Commands.Quizzes;

public class UpdateQuizStartedCommand : IRequest<bool>, ISecuredRequest<UserTypes>
{
    public string Id { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Student];
}

public class UpdateQuizStartedCommandHandler(ICommonService commonService,
                                             IQuizDal quizDal) : IRequestHandler<UpdateQuizStartedCommand, bool>
{
    public async Task<bool> Handle(UpdateQuizStartedCommand request, CancellationToken cancellationToken)
    {
        var userId = commonService.HttpUserId;
        var date = DateTime.Now;

        var quiz = await quizDal.GetAsync(
            predicate: x => x.Id == request.Id && x.UserId == userId,
            cancellationToken: cancellationToken);

        await QuizRules.QuizShouldExists(quiz);

        quiz.UpdateUser = userId;
        quiz.UpdateDate = date;
        quiz.Status = QuizStatus.Started;

        await quizDal.UpdateAsync(quiz, cancellationToken: cancellationToken);

        return true;
    }
}

public class UpdateQuizStartedCommandValidator : AbstractValidator<UpdateQuizStartedCommand>
{
    public UpdateQuizStartedCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Id).NotEmpty().WithMessage(Strings.InvalidValue);
    }
}