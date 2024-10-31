using Business.Features.Lessons.Models.Groups;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Lessons.Queries.Groups;

public class GetGroupsQuery : IRequest<PageableModel<GetGroupModel>>, ISecuredRequest<UserTypes>
{
    public PageRequest PageRequest { get; set; }

    public UserTypes[] Roles { get; } = [];
}

public class GetGroupsQueryHandler(IMapper mapper,
                                   IGroupDal groupDal) : IRequestHandler<GetGroupsQuery, PageableModel<GetGroupModel>>
{
    public async Task<PageableModel<GetGroupModel>> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();

        var groups = await groupDal.GetPageListAsyncAutoMapper<GetGroupModel>(
            enableTracking: false,
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            orderBy: x => x.OrderBy(x => x.SortNo).ThenBy(x=>x.CreateDate),
            include: x => x.Include(u => u.LessonGroups).ThenInclude(u => u.Lesson),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);
        var result = mapper.Map<PageableModel<GetGroupModel>>(groups);
        return result;
    }
}