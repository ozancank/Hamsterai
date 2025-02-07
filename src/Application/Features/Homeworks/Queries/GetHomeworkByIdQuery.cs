using Application.Features.Homeworks.Models;
using Application.Features.Homeworks.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Homeworks.Queries;

public class GetHomeworkByIdQuery : IRequest<GetHomeworkModel>, ISecuredRequest<UserTypes>
{
    public required string Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.Teacher];
    public bool AllowByPass => false;
}

public class GetHomeworkByIdHandler(IMapper mapper,
                                    IHomeworkDal homeworkDal,
                                    ICommonService commonService) : IRequestHandler<GetHomeworkByIdQuery, GetHomeworkModel>
{
    public async Task<GetHomeworkModel> Handle(GetHomeworkByIdQuery request, CancellationToken cancellationToken)
    {
        var connectionId = commonService.HttpConnectionId;
        var userType = commonService.HttpUserType;

        var result = await homeworkDal.GetAsyncAutoMapper<GetHomeworkModel>(
            enableTracking: request.Tracking,
            predicate: x => x.IsActive && x.Id == request.Id && (userType != UserTypes.Teacher || x.TeacherId == connectionId),
            include: x => x.Include(u => u.User)
                           .Include(u => u.School)
                           .Include(u => u.Teacher)
                           .Include(u => u.Lesson)
                           .Include(u => u.ClassRoom)
                           .Include(u => u.HomeworkStudents).ThenInclude(u => u.Student)
                           .Include(u => u.HomeworkUsers).ThenInclude(u => u.User),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);
        if (request.ThrowException) await HomeworkRules.HomeworkShouldExists(result);
        return result;
    }
}