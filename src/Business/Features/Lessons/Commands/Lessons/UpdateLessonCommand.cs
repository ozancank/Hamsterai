using Business.Features.Lessons.Models.Lessons;
using Business.Features.Lessons.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Lessons.Commands.Lessons;

public class UpdateLessonCommand : IRequest<GetLessonModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public UpdateLessonModel UpdateLessonModel { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public string[] HidePropertyNames { get; } = [];
}

public class UpdateLessonCommandHandler(IMapper mapper,
                                        ILessonDal lessonDal,
                                        ICommonService commonService,
                                        LessonRules lessonRules) : IRequestHandler<UpdateLessonCommand, GetLessonModel>
{
    public async Task<GetLessonModel> Handle(UpdateLessonCommand request, CancellationToken cancellationToken)
    {
        var lesson = await lessonDal.GetAsync(x => x.Id == request.UpdateLessonModel.Id, cancellationToken: cancellationToken);

        await lessonRules.LessonNameCanNotBeDuplicated(request.UpdateLessonModel.Name, request.UpdateLessonModel.Id);

        mapper.Map(request.UpdateLessonModel, lesson);
        lesson.UpdateUser = commonService.HttpUserId;
        lesson.UpdateDate = DateTime.Now;

        var updated = await lessonDal.UpdateAsyncCallback(lesson);
        var result = mapper.Map<GetLessonModel>(updated);
        return result;
    }
}

public class UpdateLessonCommandValidator : AbstractValidator<UpdateLessonModel>
{
    public UpdateLessonCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Id).NotEmpty().WithMessage(Strings.IdNotEmpty);

        RuleFor(x => x.Name).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Name).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Name).MaximumLength(50).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "50"]);

        RuleFor(x => x.SortNo).GreaterThan((byte)0).WithMessage(Strings.DynamicGratherThan, [Strings.SortNo, "0"]);
    }
}