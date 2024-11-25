using Application.Features.Lessons.Models.Lessons;
using Application.Features.Lessons.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Lessons.Queries.Lessons;

public class GetLessonByIdQuery : IRequest<GetLessonModel>, ISecuredRequest<UserTypes>
{
    public short Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetLessonByIdQueryHandler(IMapper mapper,
                                       ICommonService commonService,
                                       ILessonDal lessonDal) : IRequestHandler<GetLessonByIdQuery, GetLessonModel>
{
    public async Task<GetLessonModel> Handle(GetLessonByIdQuery request, CancellationToken cancellationToken)
    {
        var lesson = await lessonDal.GetAsyncAutoMapper<GetLessonModel>(
            enableTracking: request.Tracking,
            predicate: commonService.HttpUserType == UserTypes.Administator
                       ? x => x.Id == request.Id
                       : commonService.HttpUserType == UserTypes.Person
                         ? x => x.IsActive && x.Id == request.Id && x.RPackageLessons.Any(a => a.IsActive && a.Package != null && a.Package.PackageUsers.Any(p => p.IsActive && p.User != null && p.User.IsActive && p.UserId == commonService.HttpUserId))
                         : x => x.IsActive && x.Id == request.Id && x.RPackageLessons.Any(a => a.IsActive && a.Package != null && a.Package.PackageUsers.Any(p => p.IsActive && p.User != null && p.User.IsActive && p.User.School != null && p.User.School.IsActive && p.User.School.Id == commonService.HttpSchoolId)),
            include: x => x.Include(u => u.TeacherLessons).ThenInclude(u => u.Teacher)
                           .Include(u => u.RPackageLessons).ThenInclude(u => u.Package),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await LessonRules.LessonShouldExists(lesson);
        return lesson;
    }
}