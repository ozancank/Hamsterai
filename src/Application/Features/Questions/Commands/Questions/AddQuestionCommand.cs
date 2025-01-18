using Application.Features.Lessons.Rules;
using Application.Features.Questions.Models.Questions;
using Application.Features.Questions.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Questions.Commands.Questions;

public class AddQuestionCommand : IRequest<GetQuestionModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required AddQuestionModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.Student, UserTypes.Person];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } =
        [
            $"{nameof(Model)}.{nameof(Model.QuestionPictureBase64)}",
            $"{nameof(Model)}.{nameof(Model.QuestionSmallPictureBase64)}"
        ];
}

public class AddQuestionCommandHandler(IMapper mapper,
                                       IQuestionDal questionDal,
                                       ICommonService commonService,
                                       ILessonDal lessonDal,
                                       QuestionRules questionRules) : IRequestHandler<AddQuestionCommand, GetQuestionModel>
{
    public async Task<GetQuestionModel> Handle(AddQuestionCommand request, CancellationToken cancellationToken)
    {
        await questionRules.QuestionLimitControl();
        await questionRules.UserShouldHaveCredit(commonService.HttpUserId);

        var lessonName = await lessonDal.GetAsync(
            predicate: x => x.Id == request.Model.LessonId,
            enableTracking: false,
            selector: x => x.Name,
            cancellationToken: cancellationToken);
        await LessonRules.LessonShouldExists(lessonName);

        var id = Guid.NewGuid();
        var date = DateTime.Now;
        var userId = commonService.HttpUserId;
        var extension = Path.GetExtension(request.Model.QuestionPictureFileName);
        var fileName = $"Q_{userId}_{request.Model.LessonId}_{id}{extension}";
        await commonService.PictureConvert(request.Model.QuestionPictureBase64, fileName, AppOptions.QuestionPictureFolderPath, cancellationToken);
        await commonService.PictureConvert(request.Model.QuestionSmallPictureBase64.IfNullEmptyString(request.Model.QuestionPictureBase64), fileName, AppOptions.QuestionSmallPictureFolderPath, cancellationToken);
        await commonService.PictureConvertWithResize(request.Model.QuestionPictureBase64, fileName, AppOptions.QuestionThumbnailFolderPath, 128, cancellationToken);

        var question = new Question
        {
            Id = id,
            IsActive = true,
            CreateDate = date,
            CreateUser = userId,
            UpdateDate = date,
            UpdateUser = userId,
            LessonId = request.Model.LessonId,
            QuestionText = string.Empty,
            QuestionPictureFileName = fileName,
            QuestionPictureExtension = extension,
            AnswerText = string.Empty,
            AnswerPictureFileName = string.Empty,
            AnswerPictureExtension = string.Empty,
            Status = request.Model.IsVisual ? QuestionStatus.WaitingForOcr : QuestionStatus.Waiting,
            IsRead = false,
            ReadDate = AppStatics.MilleniumDate,
            SendForQuiz = false,
            SendQuizDate = AppStatics.MilleniumDate,
            TryCount = 0,
            GainId = null,
            RightOption = null,
            ExcludeQuiz = false,
            ExistsVisualContent = request.Model.IsVisual,
            Type = request.Model.Type.IfValue(QuestionType.None, QuestionType.Question),
        };

        var added = await questionDal.AddAsyncCallback(question, cancellationToken: cancellationToken);
        var result = mapper.Map<GetQuestionModel>(added);
        return result;
    }
}

public class AddQuestionCommandValidator : AbstractValidator<AddQuestionCommand>
{
    public AddQuestionCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.LessonId).InclusiveBetween((byte)1, (byte)255).WithMessage(Strings.DynamicBetween, [Strings.Lesson, "1", "255"]);

        RuleFor(x => x.Model.QuestionPictureBase64).MustBeValidBase64().WithMessage(Strings.DynamicNotEmpty, [Strings.Question]);

        RuleFor(x => x.Model.QuestionPictureFileName).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.FileName]);
        RuleFor(x => x.Model.QuestionPictureFileName).Must(x => x.EmptyOrTrim().Contains('.')).WithMessage(Strings.FileNameExtension);
    }
}