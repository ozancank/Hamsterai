using Business.Features.Schools.Models.Schools;
using Business.Features.Schools.Rules;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Schools.Queries.Schools;

public class GetSchoolByTaxNumberQuery : IRequest<GetSchoolModel?>, ISecuredRequest<UserTypes>
{
    public required string TaxNumber { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
}

public class GetSchoolByTaxNumberQueryHandler(IMapper mapper,
                                              IUserDal userDal,
                                              ISchoolDal schoolDal) : IRequestHandler<GetSchoolByTaxNumberQuery, GetSchoolModel?>
{
    public async Task<GetSchoolModel?> Handle(GetSchoolByTaxNumberQuery request, CancellationToken cancellationToken)
    {
        var school = await schoolDal.GetAsyncAutoMapper<GetSchoolModel>(
            enableTracking: request.Tracking,
            predicate: x => x.TaxNumber == request.TaxNumber,
            include: x => x.Include(u => u.Users)
                           .Include(x => x.RPackageSchools).ThenInclude(u => u.Package),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await SchoolRules.SchoolShouldExists(school);

        if (school != null)
            school.UserId = (await userDal.GetAsync(x => x.Type == UserTypes.School && x.SchoolId == school.Id, enableTracking: false, cancellationToken: cancellationToken))?.Id ?? 0;

        return school;
    }
}