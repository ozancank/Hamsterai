using Business.Features.Questions.Models.Quizzes;
using Business.Features.Questions.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Questions.Commands.Quizzes;

public class UpdateQuizCommand : IRequest<GetQuizModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required UpdateQuizModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Student];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class UpdateQuizCommandHandler(IMapper mapper,
                                      ICommonService commonService,
                                      IQuizDal quizDal,
                                      IQuizQuestionDal quizQuestionDal) : IRequestHandler<UpdateQuizCommand, GetQuizModel>
{
    public async Task<GetQuizModel> Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
    {
        var userId = commonService.HttpUserId;
        var date = DateTime.Now;

        var quiz = await quizDal.GetAsync(
            predicate: x => x.Id == request.Model.QuizId && x.UserId == userId,
            cancellationToken: cancellationToken);

        await QuizRules.QuizShouldExists(quiz);

        var questions = await quizQuestionDal.GetListAsync(
            predicate: x => x.QuizId == quiz.Id,
            cancellationToken: cancellationToken);

        foreach (var answer in request.Model.Answers)
        {
            var question = questions.Find(x => x.Id == answer.QuestionId);
            await QuestionRules.QuestionShouldExists(question);
            question!.UpdateUser = userId;
            question.UpdateDate = date;
            question.AnswerOption = answer.AnswerOption;
        }

        quiz.UpdateUser = userId;
        quiz.UpdateDate = date;
        quiz.TimeSecond = request.Model.TimeSecond;
        quiz.Status = request.Model.Status;
        quiz.CorrectCount = (byte)questions.Count(x => x.RightOption == x.AnswerOption);
        quiz.WrongCount = (byte)questions.Count(x => x.AnswerOption.HasValue && x.AnswerOption != ' ' && x.RightOption != x.AnswerOption);
        quiz.EmptyCount = (byte)questions.Count(x => !x.AnswerOption.HasValue || x.AnswerOption == ' ');
        quiz.SuccessRate = Math.Round(quiz.CorrectCount * 100.0 / questions.Count, 2);

        var transaction = await quizDal.CreateTransactionAsync(cancellationToken: cancellationToken);
        try
        {
            await quizQuestionDal.UpdateRangeAsync([.. questions], cancellationToken: cancellationToken);
            await quizDal.UpdateAsync(quiz, cancellationToken: cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        var result = await quizDal.GetAsyncAutoMapper<GetQuizModel>(
            predicate: x => x.Id == quiz.Id,
            enableTracking: false,
            include: x => x.Include(u => u.Lesson)
                           .Include(u => u.User).ThenInclude(u => u!.School)
                           .Include(u => u.QuizQuestions.OrderBy(x => x.SortNo)).ThenInclude(u => u.Gain),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        return result;
    }
}

public class UpdateQuizCommandValidator : AbstractValidator<UpdateQuizCommand>
{
    public UpdateQuizCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.QuizId).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.TimeSecond).GreaterThanOrEqualTo(0).WithMessage(Strings.DynamicGratherThanOrEqual, [Strings.Second, "0"]);

        RuleFor(x => x.Model.Status).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Status]);
    }
}