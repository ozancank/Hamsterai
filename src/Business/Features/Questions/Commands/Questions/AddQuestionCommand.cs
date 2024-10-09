using Business.Features.Lessons.Rules;
using Business.Features.Questions.Models.Questions;
using Business.Features.Questions.Rules;
using Business.Services.CommonService;
using Infrastructure.AI;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Questions.Commands.Questions;

public class AddQuestionCommand : IRequest<GetQuestionModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public AddQuestionModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.Student];
    public string[] HidePropertyNames { get; } = ["Model.QuestionPictureBase64"];
}

public class AddQuestionCommandHandler(IMapper mapper,
                                       IQuestionDal questionDal,
                                       ICommonService commonService,
                                       ILessonDal lessonDal,
                                       IQuestionApi questionApi,
                                       QuestionRules questionRules) : IRequestHandler<AddQuestionCommand, GetQuestionModel>
{
    public async Task<GetQuestionModel> Handle(AddQuestionCommand request, CancellationToken cancellationToken)
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
        await commonService.PictureConvert(request.Model.QuestionPictureBase64, fileName, AppOptions.QuestionPictureFolderPath);

        var question = new Question
        {
            Id = id,
            CreateDate = date,
            CreateUser = userId,
            UpdateDate = date,
            UpdateUser = userId,
            IsActive = true,
            LessonId = request.Model.LessonId,
            QuestionPictureBase64 = request.Model.QuestionPictureBase64,
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
            RightOption = null
        };

        var added = await questionDal.AddAsyncCallback(question, cancellationToken: cancellationToken);
        var result = mapper.Map<GetQuestionModel>(added);

        _ = questionApi.AskQuestionOcrImage(new()
        {
            Id = result.Id,
            Base64 = question.QuestionPictureBase64,
            LessonName = lessonName,
            UserId = question.CreateUser
        });

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
        RuleFor(x => x.Model.QuestionPictureFileName).Must(x => x.Contains('.')).WithMessage(Strings.FileNameExtension);
    }
}