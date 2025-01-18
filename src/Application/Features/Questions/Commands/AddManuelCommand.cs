using Application.Features.Lessons.Rules;
using Application.Features.Questions.Models;
using Application.Features.Questions.Models.Quizzes;
using Application.Features.Questions.Rules;
using Application.Features.Users.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Questions.Commands;

public class AddManuelCommand : IRequest<object?>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required AddManuelModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class AddManuelCommandHandler(IQuestionDal questionDal,
                                     IMapper mapper,
                                     ISimilarDal similarDal,
                                     ICommonService commonService,
                                     ILessonDal lessonDal,
                                     QuestionRules questionRules,
                                     UserRules userRules,
                                     QuizRules quizRules,
                                     IQuizDal quizDal,
                                     IQuizQuestionDal quizQuestionDal) : IRequestHandler<AddManuelCommand, object?>
{
    public async Task<object?> Handle(AddManuelCommand request, CancellationToken cancellationToken)
    {
        await questionRules.QuestionLimitControl();

        var id = Guid.NewGuid();
        var date = DateTime.Now;
        var userId = request.Model.UserId;
        var lessonId = request.Model.LessonId;
        var extension = ".png";

        await userRules.UserShouldExistsById(userId);

        var lessonName = await lessonDal.GetAsync(
            predicate: x => x.Id == lessonId,
            enableTracking: false,
            selector: x => x.Name,
            cancellationToken: cancellationToken);
        await LessonRules.LessonShouldExists(lessonName);

        switch (request.Model.QuestionType)
        {
            case QuestionType.Question:
                {
                    var requestQuestion = request.Model.Questions.First();
                    var fileNameQuestion = $"Q_{userId}_{lessonId}_{id}{extension}";
                    var fileNameAnswer = $"A_{userId}_{lessonId}_{id}{extension}";
                    await commonService.TextToImage(requestQuestion.QuestionText, fileNameQuestion, AppOptions.QuestionPictureFolderPath, cancellationToken: cancellationToken);
                    await commonService.TextToImage(requestQuestion.QuestionText, fileNameQuestion, AppOptions.QuestionSmallPictureFolderPath, cancellationToken: cancellationToken);
                    await commonService.TextToImage(requestQuestion.AnswerText, fileNameAnswer, AppOptions.AnswerPictureFolderPath, cancellationToken: cancellationToken);

                    var question = new Question
                    {
                        Id = id,
                        IsActive = true,
                        CreateDate = date,
                        CreateUser = userId,
                        UpdateDate = date,
                        UpdateUser = userId,
                        LessonId = lessonId,
                        QuestionText = requestQuestion.QuestionText,
                        QuestionPictureFileName = fileNameQuestion,
                        QuestionPictureExtension = extension,
                        AnswerText = requestQuestion.AnswerText,
                        AnswerPictureFileName = fileNameAnswer,
                        AnswerPictureExtension = extension,
                        Status = QuestionStatus.Answered,
                        IsRead = true,
                        ReadDate = date,
                        SendForQuiz = true,
                        SendQuizDate = date,
                        TryCount = 0,
                        GainId = null,
                        RightOption = requestQuestion.RightOption,
                        ExcludeQuiz = request.Model.ExcludeQuiz,
                        ExistsVisualContent = request.Model.ExistsVisualContent,
                        OcrMethod = string.Empty,
                        ErrorDescription = string.Empty,
                        AIIP = string.Empty
                    };

                    var result = await questionDal.AddAsyncCallback(question, cancellationToken: cancellationToken);
                    return result;
                }
            case QuestionType.Similar:
                {
                    var requestQuestion = request.Model.Questions.First();
                    var fileNameSimilarQuestion = $"RQ_{userId}_{lessonId}_{id}{extension}";
                    var fileNameSimilarAnswer = $"RA_{userId}_{lessonId}_{id}{extension}";
                    await commonService.TextToImage(requestQuestion.QuestionText, fileNameSimilarQuestion, AppOptions.SimilarQuestionPictureFolderPath, cancellationToken: cancellationToken);
                    await commonService.TextToImage(requestQuestion.AnswerText, fileNameSimilarAnswer, AppOptions.SimilarAnswerPictureFolderPath, cancellationToken: cancellationToken);
                    var similar = new Similar
                    {
                        Id = id,
                        IsActive = true,
                        CreateDate = date,
                        CreateUser = userId,
                        UpdateDate = date,
                        UpdateUser = userId,
                        LessonId = lessonId,
                        ResponseQuestion = requestQuestion.QuestionText,
                        ResponseQuestionFileName = fileNameSimilarQuestion,
                        ResponseQuestionExtension = extension,
                        ResponseAnswer = requestQuestion.AnswerText,
                        ResponseAnswerFileName = fileNameSimilarAnswer,
                        ResponseAnswerExtension = extension,
                        Status = QuestionStatus.Answered,
                        IsRead = true,
                        ReadDate = date,
                        SendForQuiz = true,
                        SendQuizDate = date,
                        TryCount = 0,
                        GainId = null,
                        RightOption = requestQuestion.RightOption,
                        ExcludeQuiz = request.Model.ExcludeQuiz,
                        ExistsVisualContent = request.Model.ExistsVisualContent,
                        OcrMethod = string.Empty,
                        ErrorDescription = string.Empty,
                        AIIP = string.Empty
                    };
                    var result = await similarDal.AddAsyncCallback(similar, cancellationToken: cancellationToken);
                    return result;
                }
            case QuestionType.Quiz:
                {
                    var idPrefix = $"T-{lessonId}-{userId}-";
                    var quizId = $"{idPrefix}{date:yyddMMHHmmssfff}";
                    await quizRules.QuizShouldNotExistsById(quizId);

                    var quiz = new Quiz
                    {
                        Id = quizId,
                        IsActive = true,
                        CreateDate = date,
                        CreateUser = commonService.HttpUserId,
                        UpdateDate = date,
                        UpdateUser = commonService.HttpUserId,
                        LessonId = lessonId,
                        UserId = userId,
                        TimeSecond = 0,
                        Status = QuizStatus.NotStarted,
                        CorrectCount = 0,
                        WrongCount = 0,
                        EmptyCount = 0,
                        SuccessRate = 0,
                    };

                    var questions = new List<QuizQuestion>();

                    for (byte i = 0; i < request.Model.Questions.Count; i++)
                    {
                        var requestQuestion = request.Model.Questions[i];
                        var sortNo = (byte)(i + 1);

                        var questionId = $"{quizId}-{sortNo}";
                        var fileName = $"{userId}_{lessonId}_{questionId}{extension}";
                        var questionFileName = $"TQ_{fileName}";
                        var answerFileName = $"TA_{fileName}";
                        await commonService.TextToImage(requestQuestion.QuestionText, questionFileName, AppOptions.QuizQuestionPictureFolderPath, cancellationToken: cancellationToken);
                        await commonService.TextToImage(requestQuestion.AnswerText, answerFileName, AppOptions.QuizAnswerPictureFolderPath, cancellationToken: cancellationToken);

                        var question = new QuizQuestion
                        {
                            Id = questionId,
                            IsActive = true,
                            CreateDate = date,
                            CreateUser = userId,
                            UpdateDate = date,
                            UpdateUser = userId,
                            QuizId = quizId,
                            SortNo = sortNo,
                            Question = requestQuestion.QuestionText,
                            QuestionPictureFileName = questionFileName,
                            QuestionPictureExtension = extension,
                            Answer = requestQuestion.AnswerText,
                            AnswerPictureFileName = answerFileName,
                            AnswerPictureExtension = extension,
                            RightOption = requestQuestion.RightOption ?? 'A',
                            AnswerOption = null,
                            OptionCount = requestQuestion.OptionCount,
                            GainId = null
                        };
                        questions.Add(question);
                    }

                    var result = await quizDal.ExecuteWithTransactionAsync(async () =>
                    {
                        await quizDal.AddAsync(quiz, cancellationToken: cancellationToken);
                        await quizQuestionDal.AddRangeAsync(questions, cancellationToken: cancellationToken);
                        var result = await quizDal.GetAsyncAutoMapper<GetQuizModel>(
                            predicate: x => x.Id == quizId,
                            enableTracking: false,
                            include: x => x.Include(u => u.Lesson)
                                           .Include(u => u.User).ThenInclude(u => u!.School)
                                           .Include(u => u.QuizQuestions.OrderBy(x => x.SortNo)).ThenInclude(u => u.Gain),
                            configurationProvider: mapper.ConfigurationProvider,
                            cancellationToken: cancellationToken);
                        return result;
                    }, cancellationToken: cancellationToken);
                    return result;
                }
            default:
                break;
        }

        return null;
    }
}

public class AddManuelCommandValidator : AbstractValidator<AddManuelCommand>
{
    public AddManuelCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.QuestionType).IsInEnum().WithMessage(Strings.DynamicNotEmpty, [$"{Strings.Question} {Strings.OfType}"]);

        RuleFor(x => x.Model.UserId).GreaterThan(0).WithMessage(Strings.DynamicGreaterThan, [Strings.User, "0"]);

        RuleFor(x => x.Model.LessonId).InclusiveBetween((short)1, short.MaxValue).WithMessage(Strings.DynamicBetween, [Strings.Lesson, "1", short.MaxValue.ToString()]);

        RuleForEach(x => x.Model.Questions).SetValidator(new AddManuelQuestionModelValidator());
    }
}

internal class AddManuelQuestionModelValidator : AbstractValidator<AddManuelQuestionModel>
{
    public AddManuelQuestionModelValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.QuestionText).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Question]);

        RuleFor(x => x.AnswerText).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Answer]);

        RuleFor(x => x.OptionCount).InclusiveBetween((byte)1, (byte)5).WithMessage(Strings.DynamicBetween, [$"{Strings.Option} {Strings.OfCount}", "1", "5"]);
    }
}