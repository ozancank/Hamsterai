using Business.Features.Lessons.Models.Groups;
using Business.Features.Lessons.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Lessons.Commands.Groups;

public class UpdateGroupCommand : IRequest<GetGroupModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public UpdateGroupModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public string[] HidePropertyNames { get; } = [];
}

public class UpdateGroupCommandHandler(IMapper mapper,
                                       IGroupDal groupDal,
                                       ICommonService commonService,
                                       ILessonDal lessonDal,
                                       ILessonGroupDal lessonGroupDal,
                                       GroupRules groupRules) : IRequestHandler<UpdateGroupCommand, GetGroupModel>
{
    public async Task<GetGroupModel> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        var date = DateTime.Now;
        var userId = commonService.HttpUserId;

        var group = await groupDal.GetAsync(x => x.Id == request.Model.Id, cancellationToken: cancellationToken);

        await groupRules.GroupNameCanNotBeDuplicated(request.Model.Name, request.Model.Id);

        mapper.Map(request.Model, group);
        group.UpdateUser = userId;
        group.UpdateDate = date;

        var deleteList = await lessonGroupDal.GetListAsync(predicate: x => x.GroupId == group.Id, cancellationToken: cancellationToken);

        var lessonGroups = request.Model.LessonIds.Select(x => new LessonGroup
        {
            Id = Guid.NewGuid(),
            IsActive = true,
            CreateUser = userId,
            CreateDate = date,
            UpdateUser = userId,
            UpdateDate = date,
            GroupId = group.Id,
            LessonId = x,
        }).ToList();

        await groupDal.ExecuteWithTransactionAsync(async () =>
        {
            var updated = await groupDal.UpdateAsyncCallback(group, cancellationToken: cancellationToken);
            await lessonGroupDal.DeleteRangeAsync(deleteList, cancellationToken: cancellationToken);
            await lessonGroupDal.AddRangeAsync(lessonGroups, cancellationToken: cancellationToken);
        }, cancellationToken: cancellationToken);

        
        var result = await groupDal.GetAsyncAutoMapper<GetGroupModel>(
            enableTracking: false,
            predicate: x => x.Id == group.Id,
            include: x => x.Include(u => u.LessonGroups).ThenInclude(u => u.Lesson),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);
        return result;
    }
}

public class UpdateGroupCommandValidator : AbstractValidator<UpdateGroupModel>
{
    public UpdateGroupCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Id).NotEmpty().WithMessage(Strings.IdNotEmpty);

        RuleFor(x => x.Name).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Name).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Name).MaximumLength(50).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "50"]);
    }
}