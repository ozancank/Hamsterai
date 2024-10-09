using Business.Features.Lessons.Rules;
using Business.Features.Questions.Models.Similars;
using Business.Features.Questions.Rules;
using Business.Services.CommonService;
using Infrastructure.AI;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Questions.Commands.Similars;

public class AddSimilarCommand : IRequest<GetSimilarModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public AddSimilarModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.Student];
    public string[] HidePropertyNames { get; } = ["Model.QuestionPictureBase64"];
}

public class AddSimilarCommandHandler(IMapper mapper,
                                      ISimilarQuestionDal similarQuestionDal,
                                      ICommonService commonService,
                                      ILessonDal lessonDal,
                                      IQuestionApi questionApi,
                                      SimilarRules similarRules) : IRequestHandler<AddSimilarCommand, GetSimilarModel>
{
    public async Task<GetSimilarModel> Handle(AddSimilarCommand request, CancellationToken cancellationToken)
    {
        await similarRules.SimilarLimitControl();

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

        var question = new Similar
        {
            Id = Guid.NewGuid(),
            IsActive = true,
            CreateUser = commonService.HttpUserId,
            UpdateUser = commonService.HttpUserId,
            CreateDate = date,
            UpdateDate = date,
            LessonId = request.Model.LessonId,
            QuestionPicture = request.Model.QuestionPictureBase64,
            QuestionPictureFileName = fileName,
            QuestionPictureExtension = extension,
            ResponseQuestion = string.Empty,
            ResponseQuestionFileName = string.Empty,
            ResponseQuestionExtension = string.Empty,
            ResponseAnswer = string.Empty,
            ResponseAnswerFileName = string.Empty,
            ResponseAnswerExtension = string.Empty,
            Status = QuestionStatus.Waiting,
            IsRead = false,
            SendForQuiz = false,
            TryCount = 0,
            GainId = null,
            RightOption = null,
        };

        var added = await similarQuestionDal.AddAsyncCallback(question, cancellationToken: cancellationToken);
        var result = mapper.Map<GetSimilarModel>(added);

        _ = questionApi.GetSimilarQuestion(new()
        {
            Id = result.Id,
            Base64 = question.QuestionPicture,
            LessonName = lessonName,
            UserId = question.CreateUser
        });

        return result;
    }
}

public class AddSimilarCommandValidator : AbstractValidator<AddSimilarCommand>
{
    public AddSimilarCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.LessonId).InclusiveBetween((byte)1, (byte)255).WithMessage(Strings.DynamicBetween, [Strings.Lesson, "1", "255"]);

        RuleFor(x => x.Model.QuestionPictureBase64).MustBeValidBase64().WithMessage(Strings.DynamicNotEmpty, [Strings.Question]);

        RuleFor(x => x.Model.QuestionPictureFileName).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.FileName]);
        RuleFor(x => x.Model.QuestionPictureFileName).Must(x => x.Contains('.')).WithMessage(Strings.FileNameExtension);
    }
}