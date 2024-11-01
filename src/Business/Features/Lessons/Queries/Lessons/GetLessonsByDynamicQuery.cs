using Business.Features.Lessons.Models.Lessons;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Lessons.Queries.Lessons;

public class GetLessonsByDynamicQuery : IRequest<PageableModel<GetLessonModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required Dynamic Dynamic { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetLessonsByDynamicQueryHandler(IMapper mapper,
                                             ILessonDal lessonDal) : IRequestHandler<GetLessonsByDynamicQuery, PageableModel<GetLessonModel>>
{
    public async Task<PageableModel<GetLessonModel>> Handle(GetLessonsByDynamicQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.Dynamic ??= new Dynamic();

        var users = await lessonDal.GetPageListAsyncAutoMapperByDynamic<GetLessonModel>(
            dynamic: request.Dynamic,
            enableTracking: false,
            index: request.PageRequest.Page,
            size: request.PageRequest.PageSize,
            include: x => x.Include(u => u.TeacherLessons).ThenInclude(u => u.Teacher)
                           .Include(u => u.RPackageLessons).ThenInclude(u => u.Package),
            defaultOrderColumnName: x => x.SortNo,
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);
        var list = mapper.Map<PageableModel<GetLessonModel>>(users);
        return list;
    }
}