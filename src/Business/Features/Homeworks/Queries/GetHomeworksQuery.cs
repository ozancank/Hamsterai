using Business.Features.Homeworks.Models;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Homeworks.Queries;

public class GetHomeworksQuery : IRequest<PageableModel<GetHomeworkModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required HomeworkRequestModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Teacher];
}

public class GetHomeworksQueryHandler(IMapper mapper,
                                      IHomeworkDal homeworkDal,
                                      ICommonService commonService) : IRequestHandler<GetHomeworksQuery, PageableModel<GetHomeworkModel>>
{
    public async Task<PageableModel<GetHomeworkModel>> Handle(GetHomeworksQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.Model ??= new HomeworkRequestModel();

        if (request.Model.StartDate == null) request.Model.StartDate = DateTime.Today.AddDays(-7);
        if (request.Model.EndDate == null) request.Model.EndDate = DateTime.Today;

        var homeworks = await homeworkDal.GetPageListAsyncAutoMapper<GetHomeworkModel>(
            enableTracking: false,
            predicate: x => x.IsActive
                            && x.TeacherId == commonService.HttpConnectionId
                            && x.IsActive
                            && x.CreateDate.Date >= request.Model.StartDate.Value.Date
                            && x.CreateDate.Date <= request.Model.EndDate.Value.Date.AddDays(1).AddSeconds(-1),
            include: x => x.Include(u => u.User)
                           .Include(u => u.School)
                           .Include(u => u.Teacher)
                           .Include(u => u.Lesson)
                           .Include(u => u.ClassRoom)
                           .Include(u => u.HomeworkStudents).ThenInclude(u => u.Student),
            configurationProvider: mapper.ConfigurationProvider,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetHomeworkModel>>(homeworks);
        return result;
    }
}