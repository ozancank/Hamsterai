using Business.Features.Lessons.Rules;
using Business.Features.Questions.Models.Similars;
using Business.Services.CommonService;
using Infrastructure.AI;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Questions.Commands.Similars;

public class AddSimilarCommand : IRequest<GetSimilarModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public AddSimilarModel Model { get; set; }

    public UserTypes[] Roles { get; } = [];
    public string[] HidePropertyNames { get; } = ["Model.QuestionPictureBase64"];
}

public class AddSimilarCommandHandler(IMapper mapper,
                                      ISimilarQuestionDal similarQuestionDal,
                                      ICommonService commonService,
                                      ILessonDal lessonDal,
                                      IQuestionApi questionApi) : IRequestHandler<AddSimilarCommand, GetSimilarModel>
{
    public async Task<GetSimilarModel> Handle(AddSimilarCommand request, CancellationToken cancellationToken)
    {
        var fileName = await commonService.PictureConvert(request.Model.QuestionPictureBase64, "question.png", AppOptions.QuestionPictureFolderPath);

        var lessonName = await lessonDal.GetAsync(
            predicate: x => x.Id == request.Model.LessonId,
            enableTracking: false,
            selector: x => x.Name,
            cancellationToken: cancellationToken);
        await LessonRules.LessonShouldExists(lessonName);

        var question = mapper.Map<SimilarQuestion>(request.Model);
        question.Id = Guid.NewGuid();
        question.IsActive = true;
        question.CreateUser = question.UpdateUser = commonService.HttpUserId;
        question.CreateDate = question.UpdateDate = DateTime.Now;
        question.QuestionPicture = request.Model.QuestionPictureBase64;
        question.QuestionPictureFileName = fileName.Item1;
        question.QuestionPictureExtension = fileName.Item2;
        question.ResponseQuestion = string.Empty;
        question.ResponseQuestionFileName = string.Empty;
        question.ResponseQuestionExtension = string.Empty;
        question.ResponseAnswer = string.Empty;
        question.ResponseAnswerFileName = string.Empty;
        question.ResponseAnswerExtension = string.Empty;
        question.Status = QuestionStatus.Waiting;
        question.IsRead = false;
        question.SendForQuiz = false;
        question.TryCount = 0;
        question.GainId = null;

        var added = await similarQuestionDal.AddAsyncCallback(question);
        var result = mapper.Map<GetSimilarModel>(added);

        _ = questionApi.GetSimilarQuestion(request.Model.QuestionPictureBase64, result.Id, lessonName);

        return result;
    }
}

public class AddSimilarCommandValidator : AbstractValidator<AddSimilarModel>
{
    public AddSimilarCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.LessonId).InclusiveBetween((byte)1, (byte)255).WithMessage(Strings.DynamicBetween, [Strings.Lesson, "1", "255"]);

        RuleFor(x => x.QuestionPictureBase64).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Question]);

        RuleFor(x => x.QuestionPictureFileName).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.FileName]);
        RuleFor(x => x.QuestionPictureFileName).Must(x => x.Contains('.')).WithMessage(Strings.FileNameExtension);
    }
}