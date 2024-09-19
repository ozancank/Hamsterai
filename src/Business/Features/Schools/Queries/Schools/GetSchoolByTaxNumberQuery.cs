using Business.Features.Schools.Models.Schools;
using Business.Features.Schools.Rules;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Schools.Queries.Schools;

public class GetSchoolByTaxNumberQuery : IRequest<GetSchoolModel>, ISecuredRequest<UserTypes>
{
    public string TaxNumber { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [];
}

public class GetSchoolByTaxNumberQueryHandler(IMapper mapper,
                                              ISchoolDal schoolDal) : IRequestHandler<GetSchoolByTaxNumberQuery, GetSchoolModel>
{
    public async Task<GetSchoolModel> Handle(GetSchoolByTaxNumberQuery request, CancellationToken cancellationToken)
    {
        var school = await schoolDal.GetAsyncAutoMapper<GetSchoolModel>(
            enableTracking: request.Tracking,
            predicate: x => x.TaxNumber == request.TaxNumber,
            include: x => x.Include(u => u.Users),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await SchoolRules.SchoolShouldExists(school);
        return school;
    }
}