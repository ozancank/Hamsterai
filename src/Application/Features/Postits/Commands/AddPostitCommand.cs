using Application.Features.Lessons.Rules;
using Application.Features.Postits.Models;
using Application.Features.Postits.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Postits.Commands;

public class AddPostitCommand : IRequest<GetPostitModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required AddPostitModel Model { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [$"{nameof(Model)}.{nameof(Model.PictureBase64)}"];
}

public class AddPostitCommandHandler(IMapper mapper,
                                     IPostitDal postitDal,
                                     ICommonService commonService,
                                     LessonRules lessonRules) : IRequestHandler<AddPostitCommand, GetPostitModel>
{
    public async Task<GetPostitModel> Handle(AddPostitCommand request, CancellationToken cancellationToken)
    {
        await PostitRules.OnlyOneShouldBeFilled(request.Model.Title, request.Model.Description, request.Model.PictureBase64);
        await lessonRules.LessonShouldExistsAndActive(request.Model.LessonId);

        var id = Guid.NewGuid();
        var date = DateTime.Now;
        var userId = commonService.HttpUserId;
        var fileName = string.Empty;

        if (request.Model.PictureBase64.IsNotEmpty() && request.Model.PictureFileName.IsNotEmpty())
        {
            var extension = Path.GetExtension(request.Model.PictureFileName);
            fileName = $"PT_{userId}_{request.Model.LessonId}_{id}{extension}";
            var filePath = Path.Combine(AppOptions.PostitPictureFolderPath, fileName);
            await ImageTools.Base64ToImageFile(request.Model.PictureBase64, filePath, cancellationToken);
        }

        var postit = new Postit
        {
            Id = id,
            IsActive = true,
            CreateDate = date,
            CreateUser = userId,
            UpdateDate = date,
            UpdateUser = userId,
            LessonId = request.Model.LessonId,
            Title = request.Model.Title.EmptyOrTrim(),
            Description = request.Model.Description.EmptyOrTrim(),
            Color = request.Model.Color.EmptyOrTrim("#").IfNullEmptyString(Strings.PostitDefaultColor),
            PictureFileName = fileName,
            SortNo = 0,
        };

        var added = await postitDal.AddAsyncCallback(postit, cancellationToken: cancellationToken);
        var result = mapper.Map<GetPostitModel>(added);
        return result;
    }
}

public class AddPostitCommandValidator : AbstractValidator<AddPostitCommand>
{
    public AddPostitCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.LessonId).InclusiveBetween((short)1, short.MaxValue).WithMessage(Strings.DynamicBetween, [Strings.Lesson, "1", short.MaxValue.ToString()]);

        RuleFor(x => x.Model.SortNo).GreaterThanOrEqualTo((short)0).WithMessage(Strings.DynamicGreaterThanOrEqual, [Strings.SortNo, "0"]);
    }
}