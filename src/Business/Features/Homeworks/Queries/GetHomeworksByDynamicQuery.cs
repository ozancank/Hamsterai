using Business.Features.Homeworks.Models;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Homeworks.Queries;

public class GetHomeworksByDynamicQuery : IRequest<PageableModel<GetHomeworkModel>>, ISecuredRequest<UserTypes>
{
    public PageRequest PageRequest { get; set; }
    public Dynamic Dynamic { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School, UserTypes.Teacher];
}

public class GetHomeworksByDynamicQueryHandler(IMapper mapper,
                                               IHomeworkDal homeworkDal,
                                               ICommonService commonService) : IRequestHandler<GetHomeworksByDynamicQuery, PageableModel<GetHomeworkModel>>
{
    public async Task<PageableModel<GetHomeworkModel>> Handle(GetHomeworksByDynamicQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.Dynamic ??= new Dynamic();

        var homeworks = await homeworkDal.GetPageListAsyncAutoMapperByDynamic<GetHomeworkModel>(
            dynamic: request.Dynamic,
            predicate: x => commonService.HttpUserType == UserTypes.Administator
                            || (commonService.HttpUserType == UserTypes.School && x.SchoolId == commonService.HttpSchoolId)
                            || (commonService.HttpUserType == UserTypes.Teacher && x.TeacherId == commonService.HttpConnectionId),
            enableTracking: false,
            include: x => x.Include(u => u.User)
                           .Include(u => u.School)
                           .Include(u => u.Teacher)
                           .Include(u => u.Lesson)
                           .Include(u => u.ClassRoom)
                           .Include(u => u.HomeworkStudents).ThenInclude(u => u.Student),
            defaultOrderColumnName: x => x.CreateDate,
            defaultDescending: true,
            configurationProvider: mapper.ConfigurationProvider,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetHomeworkModel>>(homeworks);
        return result;
    }
}