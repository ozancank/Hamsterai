using Application.Features.Schools.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Schools.Commands.ClassRooms;

public class PassiveClassRoomCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public int Id { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class PassiveClassRoomCommandHandler(IClassRoomDal classRoomDal,
                                            ICommonService commonService) : IRequestHandler<PassiveClassRoomCommand, bool>
{
    public async Task<bool> Handle(PassiveClassRoomCommand request, CancellationToken cancellationToken)
    {
        var classRoom = await classRoomDal.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        await ClassRoomRules.ClassRoomShouldExists(classRoom);

        classRoom.UpdateUser = commonService.HttpUserId;
        classRoom.UpdateDate = DateTime.Now;
        classRoom.IsActive = false;

        await classRoomDal.UpdateAsync(classRoom, cancellationToken: cancellationToken);

        return true;
    }
}