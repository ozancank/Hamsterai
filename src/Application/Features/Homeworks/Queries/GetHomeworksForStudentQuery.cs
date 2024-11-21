using Application.Features.Homeworks.Models;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Homeworks.Queries;

public class GetHomeworksForStudentQuery : IRequest<PageableModel<GetHomeworkStudentModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required HomeworkRequestModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.Student, UserTypes.Person];
    public bool AllowByPass => false;
}

public class GetHomeworksForStudentQueryHandler(IMapper mapper,
                                                IHomeworkStudentDal homeworkStudentDal,
                                                IHomeworkUserDal homeworkUserDal,
                                                ICommonService commonService) : IRequestHandler<GetHomeworksForStudentQuery, PageableModel<GetHomeworkStudentModel>>
{
    public async Task<PageableModel<GetHomeworkStudentModel>> Handle(GetHomeworksForStudentQuery request, CancellationToken cancellationToken)
    {
        var userId = commonService.HttpUserId;
        var connectionId = commonService.HttpConnectionId;
        var userType = commonService.HttpUserType;

        request.PageRequest ??= new PageRequest();
        request.Model ??= new HomeworkRequestModel();

        if (request.Model.StartDate == null) request.Model.StartDate = DateTime.Today.AddDays(-7);
        if (request.Model.EndDate == null) request.Model.EndDate = DateTime.Today;

        if (userType == UserTypes.Teacher)
        {
            var homeworks = await homeworkStudentDal.GetPageListAsyncAutoMapper<GetHomeworkStudentModel>(
            enableTracking: false,
            predicate: x => x.IsActive
                            && x.StudentId == connectionId
                            && x.Homework!.IsActive
                            && x.Homework.CreateDate.Date >= request.Model.StartDate.Value.Date
                            && x.Homework.CreateDate.Date <= request.Model.EndDate.Value.Date.AddDays(1).AddSeconds(-1),
            include: x => x.Include(u => u.Student)
                           .Include(u => u.Homework).ThenInclude(u => u != null ? u.User : default)
                           .Include(u => u.Homework).ThenInclude(u => u != null ? u.School : default)
                           .Include(u => u.Homework).ThenInclude(u => u != null ? u.Teacher : default)
                           .Include(u => u.Homework).ThenInclude(u => u != null ? u.Lesson : default)
                           .Include(u => u.Homework).ThenInclude(u => u != null ? u.ClassRoom : default),
            configurationProvider: mapper.ConfigurationProvider,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);

            var result = mapper.Map<PageableModel<GetHomeworkStudentModel>>(homeworks);
            return result;
        }
        else
        {
            var homeworks = await homeworkUserDal.GetPageListAsyncAutoMapper<GetHomeworkStudentModel>(
            enableTracking: false,
            predicate: x => x.IsActive
                            && x.UserId == userId
                            && x.Homework!.IsActive
                            && x.Homework.CreateDate.Date >= request.Model.StartDate.Value.Date
                            && x.Homework.CreateDate.Date <= request.Model.EndDate.Value.Date.AddDays(1).AddSeconds(-1),
            include: x => x.Include(u => u.User)
                           .Include(u => u.Homework).ThenInclude(u => u != null ? u.User : default)
                           .Include(u => u.Homework).ThenInclude(u => u != null ? u.School : default)
                           .Include(u => u.Homework).ThenInclude(u => u != null ? u.Teacher : default)
                           .Include(u => u.Homework).ThenInclude(u => u != null ? u.Lesson : default)
                           .Include(u => u.Homework).ThenInclude(u => u != null ? u.ClassRoom : default),
            configurationProvider: mapper.ConfigurationProvider,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);

            var result = mapper.Map<PageableModel<GetHomeworkStudentModel>>(homeworks);
            return result;
        }
    }
}