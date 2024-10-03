using Business.Features.Lessons.Models.Lessons;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Lessons.Queries.Lessons;

public class GetLessonsQuery : IRequest<PageableModel<GetLessonModel>>, ISecuredRequest<UserTypes>
{
    public PageRequest PageRequest { get; set; }

    public UserTypes[] Roles { get; } = [];
}

public class GetLessonsQueryHandler(IMapper mapper,
                                    ILessonDal lessonDal,
                                    ICommonService commonService) : IRequestHandler<GetLessonsQuery, PageableModel<GetLessonModel>>
{
    public async Task<PageableModel<GetLessonModel>> Handle(GetLessonsQuery request, CancellationToken cancellationToken)
    {
        long[] userIds = [18, 19, 20];
        var lesson = await lessonDal.GetPageListAsyncAutoMapper<GetLessonModel>(
            enableTracking: false,
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            predicate: x => userIds.Contains(commonService.HttpUserId) || x.IsActive,
            include: x => x.Include(u => u.TeacherLessons).ThenInclude(u => u.Teacher)
                           .Include(u => u.LessonGroups).ThenInclude(u => u.Group),
            orderBy: x => x.OrderBy(x => x.SortNo),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);
        var result = mapper.Map<PageableModel<GetLessonModel>>(lesson);
        return result;
    }
}