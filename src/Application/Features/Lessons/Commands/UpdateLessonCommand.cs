﻿using Application.Features.Lessons.Models.Lessons;
using Application.Features.Lessons.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Lessons.Commands;

public class UpdateLessonCommand : IRequest<GetLessonModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required UpdateLessonModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class UpdateLessonCommandHandler(IMapper mapper,
                                        ILessonDal lessonDal,
                                        ICommonService commonService,
                                        LessonRules lessonRules) : IRequestHandler<UpdateLessonCommand, GetLessonModel>
{
    public async Task<GetLessonModel> Handle(UpdateLessonCommand request, CancellationToken cancellationToken)
    {
        request.Model.Name = request.Model.Name!.Trim();
        var lesson = await lessonDal.GetAsync(x => x.Id == request.Model.Id, cancellationToken: cancellationToken);

        await lessonRules.LessonNameCanNotBeDuplicated(request.Model.Name, request.Model.Id);

        mapper.Map(request.Model, lesson);
        lesson.UpdateUser = commonService.HttpUserId;
        lesson.UpdateDate = DateTime.Now;

        var updated = await lessonDal.UpdateAsyncCallback(lesson, cancellationToken: cancellationToken);
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

        RuleFor(x => x.SortNo).GreaterThan((byte)0).WithMessage(Strings.DynamicGreaterThan, [Strings.SortNo, "0"]);
    }
}