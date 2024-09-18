using Business.Features.Lessons.Models.Groups;
using Business.Features.Lessons.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Lessons.Commands.Groups;

public class AddLessonInGroupCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public AddLessonInGroupModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public string[] HidePropertyNames { get; } = [];
}

public class AddLessonInGroupCommandHandler(ILessonDal lessonDal,
                                            IGroupDal groupDal,
                                            ILessonGroupDal lessonGroupDal,
                                            ICommonService commonService,
                                            GroupRules groupRules) : IRequestHandler<AddLessonInGroupCommand, bool>
{
    public async Task<bool> Handle(AddLessonInGroupCommand request, CancellationToken cancellationToken)
    {
        await groupRules.GroupShouldExistsAndActiveById(request.Model.GroupId);

        var group = await groupDal.GetAsync(
            predicate: x => x.Id == request.Model.GroupId,
            include: x => x.Include(u => u.LessonGroups).ThenInclude(u => u.Lesson),
            cancellationToken: cancellationToken);

        await lessonGroupDal.DeleteRangeAsync(group.LessonGroups);

        var date = DateTime.Now;

        if (request.Model.LessonIds != null && request.Model.LessonIds.Count != 0)
        {
            var lessons = await lessonDal.GetListAsync(
                predicate: x => x.IsActive,
                enableTracking: false,
                cancellationToken: cancellationToken);
            await LessonRules.LessonShouldBeRecordInDatabase(request.Model.LessonIds, lessons);
            var entities = new List<LessonGroup>();

            foreach (var id in request.Model.LessonIds!)
            {
                entities.Add(new LessonGroup
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    CreateUser = commonService.HttpUserId,
                    CreateDate = date,
                    UpdateUser = commonService.HttpUserId,
                    UpdateDate = date,
                    GroupId = request.Model.GroupId,
                    LessonId = id,
                });
            }

            await lessonGroupDal.AddRangeAsync(entities);
        }
        return true;
    }
}

public class AddLessonInGroupValidator : AbstractValidator<AddLessonInGroupModel>
{
    public AddLessonInGroupValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.GroupId).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Group]);
        RuleFor(x => x.GroupId).InclusiveBetween((byte)1, (byte)255).WithMessage(Strings.DynamicBetween, [Strings.Group, "1", "255"]);

        RuleFor(x => x.LessonIds).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Lesson]);
        RuleForEach(x => x.LessonIds).InclusiveBetween((byte)1, (byte)255).WithMessage(Strings.DynamicBetween, [Strings.Lesson, "1", "255"]);
    }
}