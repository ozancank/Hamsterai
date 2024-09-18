using Business.Features.Lessons.Models.Groups;
using Business.Features.Lessons.Rules;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Lessons.Queries.Groups;

public class GetGroupByIdQuery : IRequest<GetGroupModel>, ISecuredRequest<UserTypes>
{
    public byte Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [];
}

public class GetGroupByIdQueryHandler(IMapper mapper,
                                      IGroupDal groupDal) : IRequestHandler<GetGroupByIdQuery, GetGroupModel>
{
    public async Task<GetGroupModel> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await groupDal.GetAsyncAutoMapper<GetGroupModel>(
            enableTracking: request.Tracking,
            predicate: x => x.Id == request.Id,
            include: x => x.Include(u => u.LessonGroups).ThenInclude(u => u.Lesson),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await GroupRules.GroupShouldExists(group);
        return group;
    }
}