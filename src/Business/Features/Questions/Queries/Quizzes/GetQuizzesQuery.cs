using Business.Features.Questions.Models.Quizzes;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Questions.Queries.Quizzes;

public class GetQuizzesQuery : IRequest<PageableModel<GetQuizListModel>>, ISecuredRequest<UserTypes>
{
    public PageRequest PageRequest { get; set; }
    public QuizRequestModel Model { get; set; }

    public UserTypes[] Roles { get; } = [];
}

public class GetQuizzesQueryHandler(IMapper mapper,
                                    IQuizDal quizDal,
                                    ICommonService commonService) : IRequestHandler<GetQuizzesQuery, PageableModel<GetQuizListModel>>
{
    public async Task<PageableModel<GetQuizListModel>> Handle(GetQuizzesQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.Model ??= new QuizRequestModel();

        if (request.Model.StartDate == null) request.Model.StartDate = DateTime.Today.AddMonths(-1);
        if (request.Model.EndDate == null) request.Model.EndDate = DateTime.Today;

        var quizzes = await quizDal.GetPageListAsyncAutoMapper<GetQuizListModel>(
            predicate: x => x.UserId == commonService.HttpUserId && x.IsActive
                            && (request.Model.LessonId <= 0 || x.LessonId == request.Model.LessonId)
                            && x.CreateDate.Date >= request.Model.StartDate.Value.Date
                            && x.CreateDate.Date <= request.Model.EndDate.Value.Date.AddDays(1).AddSeconds(-1),
            enableTracking: false,
            include: x => x.Include(u => u.Lesson)
                           .Include(u => u.User).ThenInclude(u => u.School)
                           .Include(u => u.QuizQuestions).ThenInclude(u => u.Gain),
            orderBy: x => x.OrderByDescending(u => u.CreateDate),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);
        var result = mapper.Map<PageableModel<GetQuizListModel>>(quizzes);
        return result;
    }
}