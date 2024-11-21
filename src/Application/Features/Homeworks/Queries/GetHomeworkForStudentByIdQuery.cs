using Application.Features.Homeworks.Models;
using Application.Features.Homeworks.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Homeworks.Queries;

public class GetHomeworkForStudentByIdQuery : IRequest<GetHomeworkStudentModel>, ISecuredRequest<UserTypes>
{
    public required string Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.Student, UserTypes.Person];
    public bool AllowByPass => false;
}

public class GetHomeworkForStudentByIdHandler(IMapper mapper,
                                              IHomeworkStudentDal homeworkStudentDal,
                                              IHomeworkUserDal homeworkUserDal,
                                              ICommonService commonService) : IRequestHandler<GetHomeworkForStudentByIdQuery, GetHomeworkStudentModel>
{
    public async Task<GetHomeworkStudentModel> Handle(GetHomeworkForStudentByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = commonService.HttpUserId;
        var connectionId = commonService.HttpConnectionId;
        var userType = commonService.HttpUserType;

        if (userType == UserTypes.Teacher)
        {
            var result = await homeworkStudentDal.GetAsyncAutoMapper<GetHomeworkStudentModel>(
            enableTracking: request.Tracking,
            predicate: x => x.IsActive && x.Id == request.Id && x.StudentId == connectionId,
            include: x => x.Include(u => u.Student)
                           .Include(u => u.Homework).ThenInclude(u => u != null ? u.User : default)
                           .Include(u => u.Homework).ThenInclude(u => u != null ? u.School : default)
                           .Include(u => u.Homework).ThenInclude(u => u != null ? u.Teacher : default)
                           .Include(u => u.Homework).ThenInclude(u => u != null ? u.Lesson : default)
                           .Include(u => u.Homework).ThenInclude(u => u != null ? u.ClassRoom : default),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

            if (request.ThrowException) await HomeworkRules.HomeworkShouldExists(result);
            return result;
        }
        else
        {
            var result = await homeworkUserDal.GetAsyncAutoMapper<GetHomeworkStudentModel>(
            enableTracking: request.Tracking,
            predicate: x => x.IsActive && x.Id == request.Id && x.UserId == userId,
            include: x => x.Include(u => u.User)
                           .Include(u => u.Homework).ThenInclude(u => u != null ? u.User : default)
                           .Include(u => u.Homework).ThenInclude(u => u != null ? u.School : default)
                           .Include(u => u.Homework).ThenInclude(u => u != null ? u.Teacher : default)
                           .Include(u => u.Homework).ThenInclude(u => u != null ? u.Lesson : default)
                           .Include(u => u.Homework).ThenInclude(u => u != null ? u.ClassRoom : default),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);
            if (request.ThrowException) await HomeworkRules.HomeworkShouldExists(result);
            return result;
        }
    }
}