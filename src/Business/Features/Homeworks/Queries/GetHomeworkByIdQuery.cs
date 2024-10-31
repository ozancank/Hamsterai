using Business.Features.Homeworks.Models;
using Business.Features.Homeworks.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Homeworks.Queries;

public class GetHomeworkByIdQuery : IRequest<GetHomeworkModel>, ISecuredRequest<UserTypes>
{
    public required string Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [UserTypes.Teacher];
}

public class GetHomeworkByIdHandler(IMapper mapper,
                                    IHomeworkDal homeworkDal,
                                    ICommonService commonService) : IRequestHandler<GetHomeworkByIdQuery, GetHomeworkModel>
{
    public async Task<GetHomeworkModel> Handle(GetHomeworkByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await homeworkDal.GetAsyncAutoMapper<GetHomeworkModel>(
            enableTracking: request.Tracking,
            predicate: x => x.IsActive && x.Id == request.Id && x.TeacherId == commonService.HttpConnectionId,
            include: x => x.Include(u => u.User)
                           .Include(u => u.School)
                           .Include(u => u.Teacher)
                           .Include(u => u.Lesson)
                           .Include(u => u.ClassRoom)
                           .Include(u => u.HomeworkStudents).ThenInclude(u => u.Student),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await HomeworkRules.HomeworkShouldExists(result);
        return result;
    }
}