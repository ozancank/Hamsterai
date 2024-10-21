using Business.Features.Lessons.Rules;
using Business.Features.Questions.Models.Questions;
using Business.Features.Questions.Rules;
using Business.Services.CommonService;
using Infrastructure.OCR;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Questions.Commands.Questions;

public class AddQuestionTextCommand : IRequest<GetQuestionModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public AddQuestionModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.Student];
    public string[] HidePropertyNames { get; } = ["Model.QuestionPictureBase64"];
}

public class AddQuestionTextCommandHandler(IMapper mapper,
                                           IQuestionDal questionDal,
                                           ICommonService commonService,
                                           ILessonDal lessonDal,
                                           QuestionRules questionRules,
                                           IOcrApi ocrApi) : IRequestHandler<AddQuestionTextCommand, GetQuestionModel>
{
    public async Task<GetQuestionModel> Handle(AddQuestionTextCommand request, CancellationToken cancellationToken)
    {
        await questionRules.QuestionLimitControl();

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
        var filePath = await commonService.PictureConvert(request.Model.QuestionPictureBase64, fileName, AppOptions.QuestionPictureFolderPath);

        var ocr = await ocrApi.GetTextFromImage(new(filePath));

        var existsVisualContent = ocr.Text.Contains("##visual##", StringComparison.OrdinalIgnoreCase);

        var question = new Question
        {
            Id = id,
            IsActive = true,
            CreateDate = date,
            CreateUser = userId,
            UpdateDate = date,
            UpdateUser = userId,
            LessonId = request.Model.LessonId,
            QuestionPictureBase64 = existsVisualContent
                                    ? request.Model.QuestionPictureBase64
                                    : ocr.Text.Trim("##classic##", "##visual##"),
            QuestionPictureFileName = fileName,
            QuestionPictureExtension = extension,
            AnswerText = string.Empty,
            AnswerPictureFileName = string.Empty,
            AnswerPictureExtension = string.Empty,
            Status = QuestionStatus.Waiting,
            IsRead = false,
            SendForQuiz = false,
            TryCount = 0,
            GainId = null,
            RightOption = null,
            ExcludeQuiz = ocr.Text.Contains("##classic##", StringComparison.OrdinalIgnoreCase) || ocr.Text.Contains("Cevap X", StringComparison.OrdinalIgnoreCase),
            ExistsVisualContent = existsVisualContent
        };

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

        RuleFor(x => x.Model.LessonId).InclusiveBetween((byte)1, (byte)255).WithMessage(Strings.DynamicBetween, [Strings.Lesson, "1", "255"]);

        RuleFor(x => x.Model.QuestionPictureBase64).MustBeValidBase64().WithMessage(Strings.DynamicNotEmpty, [Strings.Question]);

        RuleFor(x => x.Model.QuestionPictureFileName).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.FileName]);
        RuleFor(x => x.Model.QuestionPictureFileName).Must(x => x.Contains('.')).WithMessage(Strings.FileNameExtension);
    }
}