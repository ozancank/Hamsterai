using Application.Features.Questions.Models.Quizzes;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Questions.Queries.Quizzes;

public class GetQuizzesQuery : IRequest<PageableModel<GetQuizListModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required QuizRequestModel Model { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetQuizzesQueryHandler(IMapper mapper,
                                    IQuizDal quizDal,
                                    ICommonService commonService) : IRequestHandler<GetQuizzesQuery, PageableModel<GetQuizListModel>>
{
    public async Task<PageableModel<GetQuizListModel>> Handle(GetQuizzesQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.PageRequest = new PageRequest(0, AppOptions.QuizMaxLimit);
        request.Model ??= new QuizRequestModel();

        //if (request.Model.StartDate == null) request.Model.StartDate = DateTime.Today.AddMonths(-1);
        //if (request.Model.EndDate == null) request.Model.EndDate = DateTime.Today;

        var quizzes = await quizDal.GetPageListAsyncAutoMapper<GetQuizListModel>(
            predicate: x => x.UserId == commonService.HttpUserId && x.IsActive
                            && (request.Model.LessonId <= 0 || x.LessonId == request.Model.LessonId),
            //&& x.CreateDate.Date >= request.Model.StartDate.Value.Date
            //&& x.CreateDate.Date <= request.Model.EndDate.Value.Date.AddDays(1).AddSeconds(-1),
            enableTracking: false,
            include: x => x.Include(u => u.Lesson)
                           .Include(u => u.User).ThenInclude(u => u!.School)
                           .Include(u => u.QuizQuestions).ThenInclude(u => u.Gain),
            orderBy: x => x.OrderByDescending(u => u.CreateDate),
            configurationProvider: mapper.ConfigurationProvider,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetQuizListModel>>(quizzes);
        result.Count = result.Count > AppOptions.QuizMaxLimit ? AppOptions.QuizMaxLimit : result.Count;
        result.HasNext = false;
        result.HasPrevious = false;
        result.Items = result.Items.Where(x => x.QuestionCount > 0);
        result.Items.ForEach(x => { x.GainNames = [.. x.GainNames.Where(x => x.IsNotEmpty()).Distinct()]; });
        return result;
    }
}