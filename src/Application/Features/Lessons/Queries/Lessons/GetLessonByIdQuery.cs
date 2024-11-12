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
        var packageId = commonService.HttpPackageId;

        var lesson = await lessonDal.GetAsyncAutoMapper<GetLessonModel>(
            enableTracking: request.Tracking,
            predicate: x => x.Id == request.Id,
            include: x => x.Include(u => u.TeacherLessons).ThenInclude(u => u.Teacher)
                           .Include(u => u.RPackageLessons).ThenInclude(u => u.Package),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await LessonRules.LessonShouldExists(lesson);
        return lesson;
    }
}