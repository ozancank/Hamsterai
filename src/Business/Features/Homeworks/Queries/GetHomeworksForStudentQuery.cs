using Business.Features.Homeworks.Models;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Homeworks.Queries;

public class GetHomeworksForStudentQuery : IRequest<PageableModel<GetHomeworkStudentModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required HomeworkRequestModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Student];
}

public class GetHomeworksForStudentQueryHandler(IMapper mapper,
                                                IHomeworkStudentDal homeworkStudentDal,
                                                ICommonService commonService) : IRequestHandler<GetHomeworksForStudentQuery, PageableModel<GetHomeworkStudentModel>>
{
    public async Task<PageableModel<GetHomeworkStudentModel>> Handle(GetHomeworksForStudentQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.Model ??= new HomeworkRequestModel();

        if (request.Model.StartDate == null) request.Model.StartDate = DateTime.Today.AddDays(-7);
        if (request.Model.EndDate == null) request.Model.EndDate = DateTime.Today;

        var homeworks = await homeworkStudentDal.GetPageListAsyncAutoMapper<GetHomeworkStudentModel>(
            enableTracking: false,
            predicate: x => x.IsActive
                            && x.StudentId == commonService.HttpConnectionId
                            && x.Homework!.IsActive
                            && x.Homework.CreateDate.Date >= request.Model.StartDate.Value.Date
                            && x.Homework.CreateDate.Date <= request.Model.EndDate.Value.Date.AddDays(1).AddSeconds(-1),
            include: x => x.Include(u => u.Student)
                           .Include(u => u.Homework).ThenInclude(u => u!.User)
                           .Include(u => u.Homework).ThenInclude(u => u!.School)
                           .Include(u => u.Homework).ThenInclude(u => u!.Teacher)
                           .Include(u => u.Homework).ThenInclude(u => u!.Lesson)
                           .Include(u => u.Homework).ThenInclude(u => u!.ClassRoom),
            configurationProvider: mapper.ConfigurationProvider,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetHomeworkStudentModel>>(homeworks);
        return result;
    }
}