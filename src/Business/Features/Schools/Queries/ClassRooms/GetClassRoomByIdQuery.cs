using Business.Features.Schools.Models.ClassRooms;
using Business.Features.Schools.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Schools.Queries.ClassRooms;

public class GetClassRoomByIdQuery : IRequest<GetClassRoomModel>, ISecuredRequest<UserTypes>
{
    public int Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [UserTypes.School];
}

public class GetClassRoomByIdQueryHandler(IMapper mapper,
                                          ICommonService commonService,
                                          IClassRoomDal classRoomDal) : IRequestHandler<GetClassRoomByIdQuery, GetClassRoomModel>
{
    public async Task<GetClassRoomModel> Handle(GetClassRoomByIdQuery request, CancellationToken cancellationToken)
    {
        var classRoom = await classRoomDal.GetAsyncAutoMapper<GetClassRoomModel>(
            enableTracking: request.Tracking,
            predicate: x => x.Id == request.Id && x.School.Id == commonService.HttpSchoolId,
            include: x => x.Include(u => u.School),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await ClassRoomRules.ClassRoomShouldExists(classRoom);
        return classRoom;
    }
}