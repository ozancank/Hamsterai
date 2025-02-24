using Application.Features.Lessons.Rules;
using Application.Features.Questions.Models.Questions;
using Application.Features.Questions.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Questions.Commands.Questions;

public class AddQuestionTextCommand : IRequest<GetQuestionModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required AddQuestionTextModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.Student, UserTypes.Person];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [$"{nameof(Model)}.{nameof(Model.QuestionText)}"];
}

public class AddQuestionTextCommandHandler(IMapper mapper,
                                       IQuestionDal questionDal,
                                       ICommonService commonService,
                                       LessonRules lessonRules,
                                       QuestionRules questionRules) : IRequestHandler<AddQuestionTextCommand, GetQuestionModel>
{
    public async Task<GetQuestionModel> Handle(AddQuestionTextCommand request, CancellationToken cancellationToken)
    {
        await questionRules.QuestionLimitControl();
        await questionRules.UserShouldHaveCredit(commonService.HttpUserId);

        var id = Guid.NewGuid();
        var date = DateTime.Now;
        var userId = commonService.HttpUserId;

        var lessonId = request.Model.LessonId;
        var questionText = request.Model.QuestionText;

        if (request.Model.QuestionId != Guid.Empty)
        {
            var exists = await questionDal.GetAsync(
                predicate: x => x.Id == request.Model.QuestionId && x.CreateUser==userId,
                enableTracking: false,
                cancellationToken: cancellationToken);
            await QuestionRules.QuestionShouldExists(exists);
            await QuestionRules.QuestionStatusShouldBeAnswered(exists.Status);

            lessonId = exists.LessonId;

            if (request.Model.Type.IsIn(QuestionType.MakeSummaryWithText, QuestionType.MakeDescriptionWithText))
                questionText = exists.AnswerText;
            else
                questionText = exists.QuestionText;
        }

        await lessonRules.LessonShouldExistsAndActive(lessonId);

        var extension = ".png";
        var fileName = $"Q_{userId}_{lessonId}_{id}{extension}";
        await commonService.TextToImage(questionText, fileName, AppOptions.AnswerPictureFolderPath, cancellationToken);
        await commonService.TextToImage(questionText, fileName, AppOptions.QuestionSmallPictureFolderPath, cancellationToken);
        await commonService.TextToImageWithResize(questionText, fileName, AppOptions.QuestionThumbnailFolderPath, 128, cancellationToken);

        var question = new Question
        {
            Id = id,
            IsActive = true,
            CreateDate = date,
            CreateUser = userId,
            UpdateDate = date,
            UpdateUser = userId,
            LessonId = lessonId,
            QuestionText = questionText,
            QuestionPictureFileName = fileName,
            QuestionPictureExtension = extension,
            AnswerText = string.Empty,
            AnswerPictureFileName = string.Empty,
            AnswerPictureExtension = string.Empty,
            Status = QuestionStatus.Waiting,
            IsRead = false,
            ReadDate = AppStatics.MilleniumDate,
            SendForQuiz = false,
            SendQuizDate = AppStatics.MilleniumDate,
            TryCount = 0,
            GainId = null,
            RightOption = null,
            ExistsVisualContent = false,
            Type = request.Model.Type,
        };
        question.ExcludeQuiz = !AppStatics.QuestionTypesForSender.Contains(question.Type);

        var added = await questionDal.AddAsyncCallback(question, cancellationToken: cancellationToken);
        var result = mapper.Map<GetQuestionModel>(added);
        return result;
    }
}

public class AddQuestionTextCommandValidator : AbstractValidator<AddQuestionTextCommand>
{
    public AddQuestionTextCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.LessonId).InclusiveBetween((short)1, short.MaxValue).When(x => x.Model.QuestionId == Guid.Empty).WithMessage(Strings.DynamicBetween, [Strings.Lesson, "1", short.MaxValue.ToString()]);

        RuleFor(x => x.Model.QuestionText).NotEmpty().When(x => x.Model.QuestionId == Guid.Empty).WithMessage(Strings.DynamicNotEmpty, [Strings.Question]);

        RuleFor(x => x.Model.Type).Must(x => AppStatics.QuestionTypesOnlyText.Contains(x)).WithMessage(Strings.QuestionTypeText);
    }
}