using Business.Features.Lessons.Rules;
using Business.Features.Teachers.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Teachers.Commands;

public class AssignLessonCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public int Id { get; set; }
    public List<byte> LessonIds { get; set; } = [];

    public UserTypes[] Roles { get; } = [UserTypes.School];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class AssignLessonCommandHandler(ITeacherDal teacherDal,
                                        ILessonDal lessonDal,
                                        IRTeacherLessonDal teacherLessonDal,
                                        ICommonService commonService) : IRequestHandler<AssignLessonCommand, bool>
{
    public async Task<bool> Handle(AssignLessonCommand request, CancellationToken cancellationToken)
    {
        var teacher = await teacherDal.GetAsync(
            predicate: x => x.Id == request.Id,
            include: x => x.Include(u => u.RTeacherLessons).ThenInclude(u => u.Lesson),
            cancellationToken: cancellationToken);

        await TeacherRules.TeacherShouldExists(request.Id);

        await teacherLessonDal.DeleteRangeAsync(teacher.RTeacherLessons, cancellationToken);

        if (request.LessonIds != null && request.LessonIds.Count != 0)
        {
            var lessons = await lessonDal.GetListAsync(enableTracking: false, cancellationToken: cancellationToken);
            await LessonRules.LessonShouldBeRecordInDatabase(request.LessonIds, lessons);
            var teacherLessons = new List<RTeacherLesson>();

            var userId = commonService.HttpUserId;
            var date = DateTime.Now;

            foreach (var id in request.LessonIds)
            {
                teacherLessons.Add(new()
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    CreateUser = userId,
                    CreateDate = DateTime.Now,
                    UpdateUser = userId,
                    UpdateDate = date,
                    TeacherId = request.Id,
                    LessonId = id
                });
            }

            await teacherLessonDal.AddRangeAsync(teacherLessons, cancellationToken: cancellationToken);
        }

        return true;
    }
}

public class AssignClaimsCommandHandlerHandlerValidator : AbstractValidator<AssignLessonCommand>
{
    public AssignClaimsCommandHandlerHandlerValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(Strings.IdNotEmpty);

        RuleFor(x => x.LessonIds).NotNull().WithMessage(Strings.LessonsNotNull);
    }
}