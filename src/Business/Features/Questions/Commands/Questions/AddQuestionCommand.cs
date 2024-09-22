using Business.Features.Lessons.Rules;
using Business.Features.Questions.Models.Questions;
using Business.Services.CommonService;
using Infrastructure.AI;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Questions.Commands.Questions;

public class AddQuestionCommand : IRequest<GetQuestionModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public AddQuestionModel Model { get; set; }

    public UserTypes[] Roles { get; } = [];
    public string[] HidePropertyNames { get; } = ["Model.QuestionPictureBase64"];
}

public class AddQuestionCommandHandler(IMapper mapper,
                                       IQuestionDal questionDal,
                                       ICommonService commonService,
                                       ILessonDal lessonDal,
                                       IQuestionApi questionApi) : IRequestHandler<AddQuestionCommand, GetQuestionModel>
{
    public async Task<GetQuestionModel> Handle(AddQuestionCommand request, CancellationToken cancellationToken)
    {
        var fileName = await commonService.PictureConvert(request.Model.QuestionPictureBase64, request.Model.QuestionPictureFileName, AppOptions.QuestionPictureFolderPath);
        var date = DateTime.Now;

        var lessonName = await lessonDal.GetAsync(
            predicate: x => x.Id == request.Model.LessonId,
            enableTracking: false,
            selector: x => x.Name,
            cancellationToken: cancellationToken);
        await LessonRules.LessonShouldExists(lessonName);

        var question = new Question
        {
            Id = Guid.NewGuid(),
            CreateDate = date,
            CreateUser = commonService.HttpUserId,
            UpdateDate = date,
            UpdateUser = commonService.HttpUserId,
            IsActive = true,
            LessonId = request.Model.LessonId,
            QuestionPictureBase64 = request.Model.QuestionPictureBase64,
            QuestionPictureFileName = fileName.Item1,
            QuestionPictureExtension = fileName.Item2,
            AnswerText = string.Empty,
            AnswerPictureFileName = string.Empty,
            AnswerPictureExtension = string.Empty,
            Status = QuestionStatus.Waiting,
            IsRead = false,
            SendForQuiz = false,
            TryCount = 0,
            GainId = null
        };

        var added = await questionDal.AddAsyncCallback(question);
        var result = mapper.Map<GetQuestionModel>(added);

        _ = questionApi.AskQuestionOcrImage(question.QuestionPictureBase64, result.Id, lessonName.Trim());

        return result;
    }
}

public class AddQuestionCommandValidator : AbstractValidator<AddQuestionModel>
{
    public AddQuestionCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.LessonId).InclusiveBetween((byte)1, (byte)255).WithMessage(Strings.DynamicBetween, [Strings.Lesson, "1", "255"]);

        RuleFor(x => x.QuestionPictureBase64).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Question]);

        RuleFor(x => x.QuestionPictureFileName).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.FileName]);
        RuleFor(x => x.QuestionPictureFileName).Must(x => x.Contains('.')).WithMessage(Strings.FileNameExtension);
    }
}