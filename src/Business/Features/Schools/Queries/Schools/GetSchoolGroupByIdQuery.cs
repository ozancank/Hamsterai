using Business.Features.Lessons.Models.Groups;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Schools.Queries.Schools;

public class GetSchoolGroupsQuery : IRequest<List<GetGroupModel>>, ISecuredRequest<UserTypes>
{
    public UserTypes[] Roles { get; } = [];
}

public class GetSchoolGroupsQueryHandler(IMapper mapper,
                                         ICommonService commonService,
                                         IGroupDal groupDal) : IRequestHandler<GetSchoolGroupsQuery, List<GetGroupModel>>
{
    public async Task<List<GetGroupModel>> Handle(GetSchoolGroupsQuery request, CancellationToken cancellationToken)
    {
        var schoolId = commonService.HttpSchoolId;

        var result = await groupDal.GetListAsyncAutoMapper<GetGroupModel>(
            enableTracking: false,
            predicate: x => x.IsActive && x.SchoolGroups.Any(s => s.SchoolId == schoolId && s.IsActive),
            include: x => x.Include(x => x.SchoolGroups),
            orderBy: x => x.OrderBy(x => x.Name),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        return result;
    }
}