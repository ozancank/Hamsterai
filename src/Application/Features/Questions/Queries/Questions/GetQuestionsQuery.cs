using Application.Features.Questions.Models.Questions;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Questions.Queries.Questions;

public class GetQuestionsQuery : IRequest<PageableModel<GetQuestionModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required QuestionRequestModel Model { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetQuestionsQueryHandler(IMapper mapper,
                                      IQuestionDal questionDal,
                                      ICommonService commonService) : IRequestHandler<GetQuestionsQuery, PageableModel<GetQuestionModel>>
{
    public async Task<PageableModel<GetQuestionModel>> Handle(GetQuestionsQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.PageRequest = new PageRequest(0, AppOptions.QuestionMaxLimit);
        request.Model ??= new QuestionRequestModel();

        //if (request.Model.StartDate == null) request.Model.StartDate = DateTime.Today.AddDays(-7);
        //if (request.Model.EndDate == null) request.Model.EndDate = DateTime.Today;

        var questions = await questionDal.GetPageListAsyncAutoMapper<GetQuestionModel>(
            predicate: x => x.CreateUser == commonService.HttpUserId && x.IsActive && (x.SendQuizDate <= DateTime.Now.AddDays(1) || !x.IsRead)
                            && (request.Model.LessonId <= 0 || x.LessonId == request.Model.LessonId)
                            && (request.Model.Type==QuestionType.None || x.Type==request.Model.Type),
            //&& x.CreateDate.Date >= request.Model.StartDate.Value.Date
            //&& x.CreateDate.Date <= request.Model.EndDate.Value.Date.AddDays(1).AddSeconds(-1),
            enableTracking: false,
            include: x => x.Include(u => u.Lesson),

            orderBy: x => x.OrderByDescending(u => u.CreateDate),
            configurationProvider: mapper.ConfigurationProvider,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetQuestionModel>>(questions);
        result.Count = result.Count > AppOptions.QuestionMaxLimit ? AppOptions.QuestionMaxLimit : result.Count;
        result.HasNext = false;
        result.HasPrevious = false;
        return result;
    }
}