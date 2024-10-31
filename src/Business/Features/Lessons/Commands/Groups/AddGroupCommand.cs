using Business.Features.Lessons.Models.Groups;
using Business.Features.Lessons.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Lessons.Commands.Groups;

public class AddGroupCommand : IRequest<GetGroupModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public AddGroupModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public string[] HidePropertyNames { get; } = [];
}

public class AddGroupCommandHandler(IMapper mapper,
    ILessonDal lessonDal,
                                     IGroupDal groupDal,
                                     ICommonService commonService,
                                     ILessonGroupDal lessonGroupDal,
                                     GroupRules groupRules) : IRequestHandler<AddGroupCommand, GetGroupModel>
{
    public async Task<GetGroupModel> Handle(AddGroupCommand request, CancellationToken cancellationToken)
    {
        await groupRules.GroupNameCanNotBeDuplicated(request.Model.Name);
        var date = DateTime.Now;

        var group = mapper.Map<Domain.Entities.Group>(request.Model);
        group.Id = await groupDal.GetNextPrimaryKeyAsync(x => x.Id, cancellationToken: cancellationToken);
        group.IsActive = true;
        group.CreateUser = group.UpdateUser = commonService.HttpUserId;
        group.CreateDate = group.UpdateDate = date;

        List<LessonGroup> lessonGroups = [];

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
                    GroupId = group.Id,
                    LessonId = id,
                });
            }
        }

        var result = await groupDal.ExecuteWithTransactionAsync(async () =>
        {
            var added = await groupDal.AddAsyncCallback(group, cancellationToken: cancellationToken);
            await lessonGroupDal.AddRangeAsync(lessonGroups, cancellationToken: cancellationToken);
            var result = mapper.Map<GetGroupModel>(added);
            return result;
        }, cancellationToken: cancellationToken);

        return result;
    }
}

public class AddGroupCommandValidator : AbstractValidator<GetGroupModel>
{
    public AddGroupCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Name).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Name).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Name).MaximumLength(50).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "50"]);
    }
}