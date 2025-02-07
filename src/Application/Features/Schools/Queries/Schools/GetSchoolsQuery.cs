using Application.Features.Schools.Models.Schools;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Schools.Queries.Schools;

public class GetSchoolsQuery : IRequest<PageableModel<GetSchoolModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
}

public class GetSchoolsQueryHandler(IMapper mapper,
                                    IUserDal userDal,
                                    ISchoolDal schoolDal) : IRequestHandler<GetSchoolsQuery, PageableModel<GetSchoolModel>>
{
    public async Task<PageableModel<GetSchoolModel>> Handle(GetSchoolsQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();

        var schools = await schoolDal.GetPageListAsyncAutoMapper<GetSchoolModel>(
            enableTracking: false,
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            include: x => x.Include(u => u.Users).ThenInclude(u => u.PackageUsers).ThenInclude(u => u.Package),
            orderBy: x => x.OrderBy(x => x.CreateDate),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        await schools.Items.ForEachAsync(async x =>
        {
            x.UserId = (await userDal.GetAsync(s => s.Type == UserTypes.School && s.SchoolId == x.Id, enableTracking: false, cancellationToken: cancellationToken))?.Id ?? 0;
        });

        var result = mapper.Map<PageableModel<GetSchoolModel>>(schools);
        return result;
    }
}