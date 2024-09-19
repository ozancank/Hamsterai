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
                                       IQuestionApi questionApi) : IRequestHandler<AddQuestionCommand, GetQuestionModel>
{
    public async Task<GetQuestionModel> Handle(AddQuestionCommand request, CancellationToken cancellationToken)
    {
        var fileName = await commonService.PictureConvert(request.Model.QuestionPictureBase64, request.Model.QuestionPictureFileName, AppOptions.QuestionPictureFolderPath);

        var question = mapper.Map<Question>(request.Model);
        question.Id = Guid.NewGuid();
        question.IsActive = true;
        question.CreateUser = question.UpdateUser = commonService.HttpUserId;
        question.CreateDate = question.UpdateDate = DateTime.Now;
        question.QuestionPictureBase64 = request.Model.QuestionPictureBase64;
        question.QuestionPictureFileName = fileName.Item1;
        question.QuestionPictureExtension = fileName.Item2;
        question.AnswerText = string.Empty;
        question.AnswerPictureFileName = string.Empty;
        question.AnswerPictureExtension = string.Empty;
        question.Status = QuestionStatus.Waiting;

        var added = await questionDal.AddAsyncCallback(question);
        var result = mapper.Map<GetQuestionModel>(added);

        _ = questionApi.AskQuestionOcrImage(question.QuestionPictureBase64, result.Id);

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