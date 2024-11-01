using Business.Features.Schools.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Schools.Commands.ClassRooms;

public class ActiveClassRoomCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public int Id { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class ActiveClassRoomCommandHandler(IClassRoomDal classRoomDal,
                                           ICommonService commonService) : IRequestHandler<ActiveClassRoomCommand, bool>
{
    public async Task<bool> Handle(ActiveClassRoomCommand request, CancellationToken cancellationToken)
    {
        var classRoom = await classRoomDal.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        await ClassRoomRules.ClassRoomShouldExists(classRoom);

        classRoom.UpdateUser = commonService.HttpUserId;
        classRoom.UpdateDate = DateTime.Now;
        classRoom.IsActive = true;

        await classRoomDal.UpdateAsync(classRoom, cancellationToken: cancellationToken);

        return true;
    }
}