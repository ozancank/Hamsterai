using Business.Features.Homeworks.Models;
using Business.Features.Homeworks.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Homeworks.Queries;

public class GetHomeworkForStudentByIdQuery : IRequest<GetHomeworkStudentModel>, ISecuredRequest<UserTypes>
{
    public string Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [UserTypes.Student];
}

public class GetHomeworkForStudentByIdHandler(IMapper mapper,
                                              IHomeworkStudentDal homeworkStudentDal,
                                              ICommonService commonService) : IRequestHandler<GetHomeworkForStudentByIdQuery, GetHomeworkStudentModel>
{
    public async Task<GetHomeworkStudentModel> Handle(GetHomeworkForStudentByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await homeworkStudentDal.GetAsyncAutoMapper<GetHomeworkStudentModel>(
            enableTracking: request.Tracking,
            predicate: x => x.IsActive && x.Id == request.Id && x.StudentId == commonService.HttpConnectionId,
            include: x => x.Include(u => u.Student)
                           .Include(u => u.Homework).ThenInclude(u => u.User)
                           .Include(u => u.Homework).ThenInclude(u => u.School)
                           .Include(u => u.Homework).ThenInclude(u => u.Teacher)
                           .Include(u => u.Homework).ThenInclude(u => u.Lesson)
                           .Include(u => u.Homework).ThenInclude(u => u.ClassRoom),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await HomeworkRules.HomeworkShouldExists(result);
        return result;
    }
}