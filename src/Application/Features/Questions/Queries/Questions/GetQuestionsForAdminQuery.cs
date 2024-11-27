using Application.Features.Questions.Models.Questions;
using Application.Features.Questions.Models.Similars;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Questions.Queries.Questions;

public class GetQuestionsForAdminQuery : IRequest<PageableModel<GetQuestionForAdminModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required QuestionRequestModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
}

public class GetQuestionsForAdminQueryHandler(IMapper mapper,
                                              IQuestionDal questionDal) : IRequestHandler<GetQuestionsForAdminQuery, PageableModel<GetQuestionForAdminModel>>
{
    public async Task<PageableModel<GetQuestionForAdminModel>> Handle(GetQuestionsForAdminQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.Model ??= new QuestionRequestModel();

        if (request.Model.StartDate == null) request.Model.StartDate = DateTime.Today.AddDays(-7);
        if (request.Model.EndDate == null) request.Model.EndDate = DateTime.Today;

        var questions = await questionDal.GetPageListAsyncAutoMapper<GetQuestionForAdminModel>(
                    enableTracking: false,
                    index: request.PageRequest.Page,
                    size: request.PageRequest.PageSize,
                    predicate: x => (request.Model.LessonId <= 0 || x.LessonId == request.Model.LessonId)
                                    && x.CreateDate.Date >= request.Model.StartDate.Value.Date
                                    && x.CreateDate.Date <= request.Model.EndDate.Value.Date.AddDays(1).AddSeconds(-1),
                    include: x => x.Include(u => u.Lesson)
                                   .Include(u => u.User)
                                   .Include(u => u.Gain),
                    orderBy: x => x.OrderByDescending(u => u.CreateDate),
                    configurationProvider: mapper.ConfigurationProvider,
                    cancellationToken: cancellationToken);
        var result = mapper.Map<PageableModel<GetQuestionForAdminModel>>(questions);

        return result;
    }
}