using Application.Features.Lessons.Rules;
using Application.Features.Questions.Models.Questions;
using Application.Features.Questions.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Questions.Commands.Questions;

public class AddRangeQuestionCommand : IRequest<List<GetQuestionModel>>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required List<AddQuestionModel> Models { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [$"{nameof(Models)}"];
}

public class AddRangeQuestionCommandHandler(IMapper mapper,
                                            IQuestionDal questionDal,
                                            ICommonService commonService,
                                            ILessonDal lessonDal,
                                            QuestionRules questionRules) : IRequestHandler<AddRangeQuestionCommand, List<GetQuestionModel>>
{
    public async Task<List<GetQuestionModel>> Handle(AddRangeQuestionCommand request, CancellationToken cancellationToken)
    {
        await questionRules.QuestionLimitControl();
        await questionRules.UserShouldHaveCredit(commonService.HttpUserId);

        var questions = new List<Question>();

        foreach (var item in request.Models)
        {
            var lessonName = await lessonDal.GetAsync(
                predicate: x => x.Id == item.LessonId,
                enableTracking: false,
                selector: x => x.Name,
                cancellationToken: cancellationToken);
            await LessonRules.LessonShouldExists(lessonName);

            var id = Guid.NewGuid();
            var date = DateTime.Now;
            var userId = commonService.HttpUserId;
            var extension = Path.GetExtension(item.QuestionPictureFileName);
            var fileName = $"Q_{userId}_{item.LessonId}_{id}{extension}";
            await commonService.PictureConvert(item.QuestionPictureBase64, fileName, AppOptions.QuestionPictureFolderPath, cancellationToken);
            await commonService.PictureConvert(item.QuestionSmallPictureBase64.IfNullEmptyString(item.QuestionPictureBase64), fileName, AppOptions.QuestionSmallPictureFolderPath, cancellationToken);
            await commonService.PictureConvertWithResize(item.QuestionPictureBase64, fileName, AppOptions.QuestionThumbnailFolderPath, 128, cancellationToken);

            var question = new Question
            {
                Id = id,
                IsActive = true,
                CreateDate = date,
                CreateUser = userId,
                UpdateDate = date,
                UpdateUser = userId,
                LessonId = item.LessonId,
                QuestionText = string.Empty,
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
                ExistsVisualContent = item.IsVisual,
                Type = item.Type.IfValue(QuestionType.None, QuestionType.Question),
            };
            question.ExcludeQuiz = !AppStatics.QuestionTypesForSender.Contains(question.Type);

            questions.Add(question);
        }

        var added = await questionDal.AddRangeAsyncCallback(questions, cancellationToken: cancellationToken);
        var result = mapper.Map<List<GetQuestionModel>>(added);

        Console.WriteLine($"Added {questions.Count} questions.");
        return result;
    }
}

public class AddRangeQuestionCommandValidator : AbstractValidator<AddRangeQuestionCommand>
{
    public AddRangeQuestionCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Models).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleForEach(x => x.Models).ChildRules(models =>
        {
            models.RuleFor(x => x.LessonId)
                  .InclusiveBetween((byte)1, (byte)255)
                  .WithMessage(Strings.DynamicBetween, [Strings.Lesson, "1", "255"]);

            models.RuleFor(x => x.QuestionPictureBase64)
                  .MustBeValidBase64()
                  .WithMessage(Strings.DynamicNotEmpty, [Strings.Question]);

            models.RuleFor(x => x.QuestionPictureFileName)
                  .NotEmpty()
                  .WithMessage(Strings.DynamicNotEmpty, [Strings.FileName]);

            models.RuleFor(x => x.QuestionPictureFileName)
                  .Must(x => x.EmptyOrTrim().Contains('.'))
                  .WithMessage(Strings.FileNameExtension);
        });
    }
}