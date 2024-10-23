using Business.Features.Schools.Models.Schools;
using Business.Features.Schools.Rules;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Schools.Queries.Schools;

public class GetSchoolByIdQuery : IRequest<GetSchoolModel>, ISecuredRequest<UserTypes>
{
    public byte Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
}

public class GetSchoolByIdQueryHandler(IMapper mapper,
                                       ISchoolDal schoolDal) : IRequestHandler<GetSchoolByIdQuery, GetSchoolModel>
{
    public async Task<GetSchoolModel> Handle(GetSchoolByIdQuery request, CancellationToken cancellationToken)
    {
        var school = await schoolDal.GetAsyncAutoMapper<GetSchoolModel>(
            enableTracking: request.Tracking,
            predicate: x => x.Id == request.Id,
            include: x => x.Include(u => u.Users)
                           .Include(x => x.SchoolGroups).ThenInclude(u => u.Group),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await SchoolRules.SchoolShouldExists(school);
        return school;
    }
}