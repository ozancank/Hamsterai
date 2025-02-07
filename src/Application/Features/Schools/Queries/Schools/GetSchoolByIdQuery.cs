using Application.Features.Schools.Models.Schools;
using Application.Features.Schools.Rules;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Schools.Queries.Schools;

public class GetSchoolByIdQuery : IRequest<GetSchoolModel?>, ISecuredRequest<UserTypes>
{
    public int Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
}

public class GetSchoolByIdQueryHandler(IMapper mapper,
                                       IUserDal userDal,
                                       ISchoolDal schoolDal) : IRequestHandler<GetSchoolByIdQuery, GetSchoolModel?>
{
    public async Task<GetSchoolModel?> Handle(GetSchoolByIdQuery request, CancellationToken cancellationToken)
    {
        var school = await schoolDal.GetAsyncAutoMapper<GetSchoolModel>(
            enableTracking: request.Tracking,
            predicate: x => x.Id == request.Id,
            include: x => x.Include(u => u.Users).ThenInclude(u=>u.PackageUsers).ThenInclude(u => u.Package),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await SchoolRules.SchoolShouldExists(school);

        if (school != null)
            school.UserId = (await userDal.GetAsync(x => x.Type == UserTypes.School && x.SchoolId == school.Id, enableTracking: false, cancellationToken: cancellationToken))?.Id ?? 0;

        return school;
    }
}