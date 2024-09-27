using Business.Features.Lessons.Rules;
using Business.Features.Questions.Models.Quizzes;
using Business.Features.Questions.Rules;
using Business.Features.Users.Rules;
using Business.Services.CommonService;
using Business.Services.GainService;
using Infrastructure.AI;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Questions.Commands.Quizzes;

public class AddQuizCommand : IRequest<GetQuizModel>, ISecuredRequest<UserTypes>
{
    public AddQuizModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
}

public class AddQuizCommandHandler(IMapper mapper,
                                   ICommonService commonService,
                                   IQuizDal quizDal,
                                   IQuizQuestionDal quizQuestionDal,
                                   ILessonDal lessonDal,
                                   IQuestionApi questionApi,
                                   IGainService gainService,
                                   UserRules userRules) : IRequestHandler<AddQuizCommand, GetQuizModel>
{
    public async Task<GetQuizModel> Handle(AddQuizCommand request, CancellationToken cancellationToken)
    {
        await QuizRules.QuizQuestionShouldExists(request.Model.Base64List);
        await userRules.UserShouldExistsAndActiveById(request.Model.UserId);

        var lessonName = await lessonDal.GetAsync(
            predicate: x => x.Id == request.Model.LessonId,
            enableTracking: false,
            selector: x => x.Name,
            cancellationToken: cancellationToken);
        await LessonRules.LessonShouldExists(lessonName);

        var responses = await questionApi.GetQuizQuestions(new()
        {
            QuestionImages = request.Model.Base64List,
            LessonName = lessonName
        });
        await QuizRules.QuizQuestionsShouldExists(responses.Questions);

        var userId = commonService.HttpUserId;
        var date = DateTime.Now;
        var count = await quizDal.CountOfRecordAsync(predicate: x => x.UserId == userId && x.LessonId == request.Model.LessonId, enableTracking: false, cancellationToken: cancellationToken) + 1;
        var quiz = new Quiz
        {
            Id = $"T-{request.Model.LessonId}-{userId}-{count}",
            IsActive = true,
            CreateUser = userId,
            CreateDate = date,
            UpdateUser = userId,
            UpdateDate = date,
            UserId = request.Model.UserId,
            LessonId = request.Model.LessonId,
            TimeSecond = 0,
            Status = QuizStatus.NotStarted,
            CorrectCount = 0,
            WrongCount = 0,
            EmptyCount = 0,
            SuccessRate = 0,
        };

        var questions = new List<QuizQuestion>();

        for (byte i = 0; i < responses.Questions.Count; i++)
        {
            var response = responses.Questions[i];
            var sortNo = (byte)(i + 1);

            var id = $"{quiz.Id}-{sortNo}";
            var extension = ".png";
            var fileName = $"{userId}_{request.Model.LessonId}_{id}{extension}";
            var questionFileName = $"TQ_{fileName}";
            var answerFileName = $"TA_{fileName}";
            await commonService.PictureConvert(response.SimilarImage, questionFileName, AppOptions.QuizQuestionPictureFolderPath);
            await commonService.PictureConvert(response.AnswerImage, answerFileName, AppOptions.QuizAnswerPictureFolderPath);

            var gain = await gainService.GetOrAddGain(new(response.GainName, request.Model.LessonId, userId));

            questions.Add(new()
            {
                Id = id,
                IsActive = true,
                CreateUser = userId,
                CreateDate = date,
                UpdateUser = userId,
                UpdateDate = date,
                QuizId = quiz.Id,
                SortNo = sortNo,
                Question = response.QuestionText,
                QuestionPictureFileName = questionFileName,
                QuestionPictureExtension = extension,
                Answer = response.AnswerText,
                AnswerPictureFileName = answerFileName,
                AnswerPictureExtension = extension,
                RightOption = response.RightOption.Trim()[0],
                AnswerOption = null,
                OptionCount = (byte)response.OptionCount,
                GainId = gain.Id
            });
        }

        await quizDal.ExecuteWithTransactionAsync(async () =>
        {
            await quizDal.AddAsync(quiz, cancellationToken: cancellationToken);
            await quizQuestionDal.AddRangeAsync(questions, cancellationToken: cancellationToken);

        }, cancellationToken: cancellationToken);

        var result = await quizDal.GetAsyncAutoMapper<GetQuizModel>(
            enableTracking: false,
            predicate: x => x.Id == quiz.Id,
            include: x => x.Include(u => u.QuizQuestions),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        return result;
    }
}

public class AddQuizCommandValidator : AbstractValidator<AddQuizCommand>
{
    public AddQuizCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.UserId).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.LessonId).InclusiveBetween((byte)1, (byte)255).WithMessage(Strings.DynamicBetween, [Strings.Lesson, "1", "255"]);

        RuleFor(x => x.Model.Base64List).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Question]);
    }
}