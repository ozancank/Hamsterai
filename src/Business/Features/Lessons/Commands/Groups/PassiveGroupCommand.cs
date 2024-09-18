using Business.Features.Lessons.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Lessons.Commands.Groups;

public class PassiveGroupCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public byte Id { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public string[] HidePropertyNames { get; } = [];
}

public class PassiveGroupCommandHandler(IGroupDal groupDal,
                                        ICommonService commonService) : IRequestHandler<PassiveGroupCommand, bool>
{
    public async Task<bool> Handle(PassiveGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await groupDal.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        await GroupRules.GroupShouldExists(group);

        group.UpdateUser = commonService.HttpUserId;
        group.UpdateDate = DateTime.Now;
        group.IsActive = false;

        await groupDal.UpdateAsync(group);
        return true;
    }
}