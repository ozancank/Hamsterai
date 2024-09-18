using Business.Features.Lessons.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Lessons.Commands.Lessons;

public class ActiveLessonCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public byte Id { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public string[] HidePropertyNames { get; } = [];
}

public class ActiveLessonCommandHandler(ILessonDal lessonDal,
                                        ICommonService commonService) : IRequestHandler<ActiveLessonCommand, bool>
{
    public async Task<bool> Handle(ActiveLessonCommand request, CancellationToken cancellationToken)
    {
        var lesson = await lessonDal.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        await LessonRules.LessonShouldExists(lesson);

        lesson.UpdateUser = commonService.HttpUserId;
        lesson.UpdateDate = DateTime.Now;
        lesson.IsActive = true;

        await lessonDal.UpdateAsync(lesson);
        return true;
    }
}