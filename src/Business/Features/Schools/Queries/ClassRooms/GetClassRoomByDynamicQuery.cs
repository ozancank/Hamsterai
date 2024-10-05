using Business.Features.Schools.Models.ClassRooms;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Schools.Queries.ClassRooms;

public class GetClassRoomsByDynamicQuery : IRequest<PageableModel<GetClassRoomModel>>, ISecuredRequest<UserTypes>
{
    public PageRequest PageRequest { get; set; }
    public Dynamic Dynamic { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School];
}

public class GetClassRoomsByDynamicQueryHandler(IMapper mapper,
                                                ICommonService commonService,
                                                IClassRoomDal classRoomDal) : IRequestHandler<GetClassRoomsByDynamicQuery, PageableModel<GetClassRoomModel>>
{
    public async Task<PageableModel<GetClassRoomModel>> Handle(GetClassRoomsByDynamicQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.Dynamic ??= new Dynamic();

        var classRooms = await classRoomDal.GetPageListAsyncAutoMapperByDynamic<GetClassRoomModel>(
            dynamic: request.Dynamic,
            predicate: x => commonService.HttpUserType == UserTypes.Administator || x.School.Id == commonService.HttpSchoolId,
            defaultOrderColumnName: x => x.CreateDate,
            enableTracking: false,
            configurationProvider: mapper.ConfigurationProvider,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);
        var list = mapper.Map<PageableModel<GetClassRoomModel>>(classRooms);
        return list;
    }
}