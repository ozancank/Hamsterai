﻿using Application.Features.Lessons.Rules;
using Application.Features.Questions.Models.Similars;
using Application.Features.Questions.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Questions.Commands.Similars;

public class AddSimilarCommand : IRequest<GetSimilarModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required AddSimilarModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.Student, UserTypes.Person];
    public bool AllowByPass => false;

    public string[] HidePropertyNames { get; } =
    [
        $"{nameof(Model)}.{nameof(Model.QuestionPictureBase64)}",
        $"{nameof(Model)}.{nameof(Model.QuestionSmallPictureBase64)}"
    ];
}

public class AddSimilarCommandHandler(IMapper mapper,
                                      ISimilarDal similarDal,
                                      ICommonService commonService,
                                      ILessonDal lessonDal,
                                      SimilarRules similarRules,
                                      QuestionRules questionRules) : IRequestHandler<AddSimilarCommand, GetSimilarModel>
{
    public async Task<GetSimilarModel> Handle(AddSimilarCommand request, CancellationToken cancellationToken)
    {
        await similarRules.SimilarLimitControl();
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

        var question = new Similar
        {
            Id = id,
            IsActive = true,
            CreateUser = commonService.HttpUserId,
            CreateDate = date,
            UpdateUser = commonService.HttpUserId,
            UpdateDate = date,
            LessonId = request.Model.LessonId,
            ResponseQuestion = string.Empty,
            ResponseQuestionFileName = string.Empty,
            ResponseQuestionExtension = string.Empty,
            ResponseAnswer = string.Empty,
            ResponseAnswerFileName = string.Empty,
            ResponseAnswerExtension = string.Empty,
            Status = QuestionStatus.Waiting,
            IsRead = false,
            ReadDate = AppStatics.MilleniumDate,
            SendForQuiz = false,
            SendQuizDate = AppStatics.MilleniumDate,
            TryCount = 0,
            GainId = null,
            RightOption = null,
            ExcludeQuiz = false,
            ExistsVisualContent = true,
        };

        var added = await similarDal.AddAsyncCallback(question, cancellationToken: cancellationToken);
        var result = mapper.Map<GetSimilarModel>(added);
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
        RuleFor(x => x.Model.QuestionPictureFileName).Must(x => x.EmptyOrTrim().Contains('.')).WithMessage(Strings.FileNameExtension);
    }
}