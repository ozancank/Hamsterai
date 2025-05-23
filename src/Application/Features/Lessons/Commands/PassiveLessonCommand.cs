﻿using Application.Features.Lessons.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Lessons.Commands;

public class PassiveLessonCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public short Id { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class PassiveLessonCommandHandler(ILessonDal lessonDal,
                                         ICommonService commonService) : IRequestHandler<PassiveLessonCommand, bool>
{
    public async Task<bool> Handle(PassiveLessonCommand request, CancellationToken cancellationToken)
    {
        var lesson = await lessonDal.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        await LessonRules.LessonShouldExists(lesson);

        lesson.UpdateUser = commonService.HttpUserId;
        lesson.UpdateDate = DateTime.Now;
        lesson.IsActive = false;

        await lessonDal.UpdateAsync(lesson, cancellationToken: cancellationToken);
        return true;
    }
}