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
                                     IGroupDal groupDal,
                                     ICommonService commonService,
                                     GroupRules groupRules) : IRequestHandler<AddGroupCommand, GetGroupModel>
{
    public async Task<GetGroupModel> Handle(AddGroupCommand request, CancellationToken cancellationToken)
    {
        await groupRules.GroupNameCanNotBeDuplicated(request.Model.Name);

        var group = mapper.Map<Group>(request.Model);
        group.Id = await groupDal.GetNextPrimaryKeyAsync(x => x.Id, cancellationToken: cancellationToken);
        group.IsActive = true;
        group.CreateUser = group.UpdateUser = commonService.HttpUserId;
        group.CreateDate = group.UpdateDate = DateTime.Now;

        var added = await groupDal.AddAsyncCallback(group);
        var result = mapper.Map<GetGroupModel>(added);
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