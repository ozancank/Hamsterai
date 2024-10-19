using Business.Features.Schools.Models.ClassRooms;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Schools.Queries.ClassRooms;

public class GetClassRoomsQuery : IRequest<PageableModel<GetClassRoomModel>>, ISecuredRequest<UserTypes>
{
    public PageRequest PageRequest { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School,UserTypes.Teacher];
}

public class GetClassRoomsQueryHandler(IMapper mapper,
                                       ICommonService commonService,
                                       IClassRoomDal classRoomDal) : IRequestHandler<GetClassRoomsQuery, PageableModel<GetClassRoomModel>>
{
    public async Task<PageableModel<GetClassRoomModel>> Handle(GetClassRoomsQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();

        var classRooms = await classRoomDal.GetPageListAsyncAutoMapper<GetClassRoomModel>(
            enableTracking: false,
            predicate: x => commonService.HttpUserType == UserTypes.Administator || x.School.Id == commonService.HttpSchoolId,
            include: x => x.Include(u => u.School),
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            orderBy: x => x.OrderBy(x => x.CreateDate),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetClassRoomModel>>(classRooms);
        return result;
    }
}