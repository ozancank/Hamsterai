using Business.Features.Students.Models;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Students.Queries;

public class GetStudentGainsForSelfQuery : IRequest<GetStudentGainsModel>, ISecuredRequest<UserTypes>
{
    public UserTypes[] Roles { get; } = [UserTypes.Student];
}

public class GetGainsForStudentIdQueryHandler(ICommonService commonService,
                                              IQuestionDal questionDal,
                                              IQuizQuestionDal quizQuestionDal,
                                              ISimilarDal similarQuestionDal) : IRequestHandler<GetStudentGainsForSelfQuery, GetStudentGainsModel>
{
    public async Task<GetStudentGainsModel> Handle(GetStudentGainsForSelfQuery request, CancellationToken cancellationToken)
    {
        var result = new GetStudentGainsModel();
        var userId = commonService.HttpUserId;

        var startDate = DateTime.Today.AddMonths(-1);
        var endDate = DateTime.Today;

        var questions = await questionDal.GetListAsync(
            enableTracking: false,
            predicate: x => x.CreateUser == userId
                            && x.Status == QuestionStatus.Answered
                            && x.GainId.HasValue
                            && x.CreateDate.Date >= startDate.Date
                            && x.CreateDate.Date <= endDate.Date.AddDays(1).AddSeconds(-1),
            include: x => x.Include(u => u.Lesson).Include(u => u.Gain),
            selector: x => new { Lesson = x.Lesson!.Name, Gain = x.Gain!.Name },
            cancellationToken: cancellationToken);

        var similarQuestions = await similarQuestionDal.GetListAsync(
            enableTracking: false,
            predicate: x => x.CreateUser == userId
                            && x.Status == QuestionStatus.Answered
                            && x.GainId.HasValue
                            && x.CreateDate.Date >= startDate.Date
                            && x.CreateDate.Date <= endDate.Date.AddDays(1).AddSeconds(-1),
            include: x => x.Include(u => u.Lesson).Include(u => u.Gain),
            selector: x => new { Lesson = x.Lesson!.Name, Gain = x.Gain!.Name },
            cancellationToken: cancellationToken);

        var quizQuestions = await quizQuestionDal.GetListAsync(
            enableTracking: false,
            predicate: x => x.CreateUser == userId
                            && x.Quiz!.Status == QuizStatus.Completed
                            && x.CreateDate.Date >= startDate.Date
                            && x.CreateDate.Date <= endDate.Date.AddDays(1).AddSeconds(-1),
            include: x => x.Include(x => x.Quiz).ThenInclude(u => u!.Lesson)
                           .Include(u => u.Gain),
            selector: x => new { Lesson = x.Quiz!.Lesson!.Name, Gain = x.Gain!.Name },
            cancellationToken: cancellationToken);

        var allQuestions = questions.Concat(similarQuestions).Concat(quizQuestions).ToList();

        result.ForLessons = allQuestions.Distinct()
            .GroupBy(x => x.Lesson)
            .Select(g => new { Lesson = g.Key, Count = g.Count() })
            .ToDictionary(x => x.Lesson!, x => x.Count);

        result.ForGains = allQuestions
            .GroupBy(x => x.Gain)
            .Select(g => new { Gain = g.Key, Count = g.Count() })
            .ToDictionary(x => x.Gain!, x => x.Count);

        result.ForLessonGains = allQuestions
            .GroupBy(x => x.Lesson)
            .Select(g => new
            {
                Lesson = g.Key,
                Gains = g.GroupBy(y => y.Gain)
                         .Select(y => new { Gain = y.Key, Count = y.Count() })
                         .ToDictionary(y => y.Gain!, y => y.Count)
            })
            .ToDictionary(x => x.Lesson!, x => x.Gains);

        result.Info = new Dictionary<string, int>
        {
            { "TotalQuestion", allQuestions.Count },
            { "TotalGain", result.ForLessons.Sum(x=>x.Value) }
        };

        return result;
    }
}