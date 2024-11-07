using Business.Features.Schools.Models.Schools;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Schools.Queries.Schools;

public class GetSchoolsByDynamicQuery : IRequest<PageableModel<GetSchoolModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required Dynamic Dynamic { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
}

public class GetSchoolsByDynamicQueryHandler(IMapper mapper,
                                             IUserDal userDal,
                                             ISchoolDal schoolDal) : IRequestHandler<GetSchoolsByDynamicQuery, PageableModel<GetSchoolModel>>
{
    public async Task<PageableModel<GetSchoolModel>> Handle(GetSchoolsByDynamicQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.Dynamic ??= new Dynamic();

        var schools = await schoolDal.GetPageListAsyncAutoMapperByDynamic<GetSchoolModel>(
            dynamic: request.Dynamic,
            defaultOrderColumnName: x => x.CreateDate,
            enableTracking: false,
            include: x => x.Include(u => u.Users)
                           .Include(x => x.RPackageSchools).ThenInclude(u => u.Package),
            configurationProvider: mapper.ConfigurationProvider,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);

        await schools.Items.ForEachAsync(async x =>
        {
            x.UserId = (await userDal.GetAsync(s => s.Type == UserTypes.School && s.SchoolId == x.Id, enableTracking: false, cancellationToken: cancellationToken))?.Id ?? 0;
        });

        var list = mapper.Map<PageableModel<GetSchoolModel>>(schools);
        return list;
    }
}