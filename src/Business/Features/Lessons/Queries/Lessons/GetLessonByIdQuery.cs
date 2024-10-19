using Business.Features.Lessons.Models.Lessons;
using Business.Features.Lessons.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Lessons.Queries.Lessons;

public class GetLessonByIdQuery : IRequest<GetLessonModel>, ISecuredRequest<UserTypes>
{
    public byte Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [];
}

public class GetLessonByIdQueryHandler(IMapper mapper,
                                       ICommonService commonService,
                                       ILessonDal lessonDal) : IRequestHandler<GetLessonByIdQuery, GetLessonModel>
{
    public async Task<GetLessonModel> Handle(GetLessonByIdQuery request, CancellationToken cancellationToken)
    {
        var groupId=commonService.HttpGroupId;       

        var lesson = await lessonDal.GetAsyncAutoMapper<GetLessonModel>(
            enableTracking: request.Tracking,
            predicate: x => x.Id == request.Id,
            include: x => x.Include(u => u.TeacherLessons).ThenInclude(u => u.Teacher)
                           .Include(u => u.LessonGroups).ThenInclude(u => u.Group),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await LessonRules.LessonShouldExists(lesson);
        return lesson;
    }
}