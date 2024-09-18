using Business.Features.Lessons.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Lessons.Commands.Groups;

public class ActiveGroupCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public byte Id { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public string[] HidePropertyNames { get; } = [];
}

public class ActiveGroupCommandHandler(IGroupDal groupDal,
                                       ICommonService commonService) : IRequestHandler<ActiveGroupCommand, bool>
{
    public async Task<bool> Handle(ActiveGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await groupDal.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        await GroupRules.GroupShouldExists(group);

        group.UpdateUser = commonService.HttpUserId;
        group.UpdateDate = DateTime.Now;
        group.IsActive = true;

        await groupDal.UpdateAsync(group);
        return true;
    }
}