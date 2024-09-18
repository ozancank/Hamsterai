using Business.Features.Questions.Models.Questions;
using Business.Features.Questions.Models.Similars;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Questions.Queries.Questions;

public class GetQuestionsQuery : IRequest<PageableModel<GetQuestionModel>>, ISecuredRequest<UserTypes>
{
    public PageRequest PageRequest { get; set; }
    public QuestionRequestModel Model { get; set; }

    public UserTypes[] Roles { get; } = [];
}

public class GetQuestionsQueryHandler(IMapper mapper,
                                      IQuestionDal questionDal,
                                      ICommonService commonService) : IRequestHandler<GetQuestionsQuery, PageableModel<GetQuestionModel>>
{
    public async Task<PageableModel<GetQuestionModel>> Handle(GetQuestionsQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.Model ??= new QuestionRequestModel();

        if (request.Model.StartDate == null) request.Model.StartDate = DateTime.Today.AddDays(-7);
        if (request.Model.EndDate == null) request.Model.EndDate = DateTime.Today;

        var questions = await questionDal.GetPageListAsyncAutoMapper<GetQuestionModel>(
            predicate: x => x.CreateUser == commonService.HttpUserId && x.IsActive
                            && (request.Model.LessonId <= 0 || x.LessonId == request.Model.LessonId)
                            && x.CreateDate.Date >= request.Model.StartDate.Value.Date
                            && x.CreateDate.Date <= request.Model.EndDate.Value.Date.AddDays(1).AddSeconds(-1),
            enableTracking: false,
            include: x => x.Include(u => u.Lesson),
            orderBy: x => x.OrderByDescending(u => u.CreateDate),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);
        var result = mapper.Map<PageableModel<GetQuestionModel>>(questions);
        return result;
    }
}