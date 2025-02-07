using Application.Features.Lessons.Rules;
using Application.Features.Postits.Models;
using Application.Features.Postits.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Postits.Commands;

public class UpdatePostitCommand : IRequest<GetPostitModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required UpdatePostitModel Model { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class UpdatePostitCommandHandler(IMapper mapper,
                                        IPostitDal postitDal,
                                        ICommonService commonService,
                                        LessonRules lessonRules) : IRequestHandler<UpdatePostitCommand, GetPostitModel>
{
    public async Task<GetPostitModel> Handle(UpdatePostitCommand request, CancellationToken cancellationToken)
    {
        var userId = commonService.HttpUserId;

        var postit = await postitDal.GetAsync(
            predicate: x => x.CreateUser == userId && x.Id == request.Model.Id,
            cancellationToken: cancellationToken);

        await lessonRules.LessonShouldExistsAndActive(request.Model.LessonId);
        await PostitRules.PostitShouldExists(postit);

        if (request.Model.RemovePicture && postit.PictureFileName.IsNotEmpty())
        {
            var filePath = Path.Combine(AppOptions.PostitPictureFolderPath, postit.PictureFileName!);
            if (File.Exists(filePath)) File.Delete(filePath);
            postit.PictureFileName = string.Empty;
        }

        await PostitRules.OnlyOneShouldBeFilled(request.Model.Title, request.Model.Description, postit.PictureFileName);

        postit.UpdateUser = userId;
        postit.UpdateDate = DateTime.Now;
        postit.LessonId = request.Model.LessonId;
        postit.Title = request.Model.Title.EmptyOrTrim();
        postit.Description = request.Model.Description.EmptyOrTrim();
        postit.Color = request.Model.Color.EmptyOrTrim("#").IfNullEmptyString(Strings.PostitDefaultColor);
        postit.SortNo = request.Model.SortNo;

        var updated = await postitDal.UpdateAsyncCallback(postit, cancellationToken: cancellationToken);
        var result = mapper.Map<GetPostitModel>(updated);
        return result;
    }
}

public class UpdatePostitCommandValidator : AbstractValidator<UpdatePostitCommand>
{
    public UpdatePostitCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.LessonId).InclusiveBetween((short)1, short.MaxValue).WithMessage(Strings.DynamicBetween, [Strings.Lesson, "1", short.MaxValue.ToString()]);

        RuleFor(x => x.Model.SortNo).GreaterThanOrEqualTo((short)0).WithMessage(Strings.DynamicGreaterThanOrEqual, [Strings.SortNo, "0"]);
    }
}