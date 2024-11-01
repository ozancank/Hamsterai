using Business.Features.Lessons.Models.Lessons;
using Business.Features.Lessons.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Lessons.Commands.Lessons;

public class AddLessonCommand : IRequest<GetLessonModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required AddLessonModel AddLessonModel { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class AddLessonCommandHandler(IMapper mapper,
                                     ILessonDal lessonDal,
                                     ICommonService commonService,
                                     LessonRules lessonRules) : IRequestHandler<AddLessonCommand, GetLessonModel>
{
    public async Task<GetLessonModel> Handle(AddLessonCommand request, CancellationToken cancellationToken)
    {
        await lessonRules.LessonNameCanNotBeDuplicated(request.AddLessonModel.Name!);

        var lesson = mapper.Map<Lesson>(request.AddLessonModel);
        lesson.Id = await lessonDal.GetNextPrimaryKeyAsync(x => x.Id, cancellationToken: cancellationToken);
        lesson.IsActive = true;
        lesson.CreateUser = lesson.UpdateUser = commonService.HttpUserId;
        lesson.CreateDate = lesson.UpdateDate = DateTime.Now;

        var added = await lessonDal.AddAsyncCallback(lesson, cancellationToken: cancellationToken);
        var result = mapper.Map<GetLessonModel>(added);

        return result;
    }
}

public class AddLessonCommandValidator : AbstractValidator<AddLessonModel>
{
    public AddLessonCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Name).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Name).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Name).MaximumLength(50).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "50"]);

        RuleFor(x => x.SortNo).GreaterThan((byte)0).WithMessage(Strings.DynamicGratherThan, [Strings.SortNo, "0"]);
    }
}