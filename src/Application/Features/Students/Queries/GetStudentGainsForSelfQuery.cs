using Application.Features.Students.Models;
using Application.Services.CommonService;
using Domain.Entities;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Students.Queries;

public class GetStudentGainsForSelfQuery : IRequest<GetStudentGainsModel>, ISecuredRequest<UserTypes>
{
    public UserTypes[] Roles { get; } = [UserTypes.Student, UserTypes.Person];
    public bool AllowByPass => false;
}

public class GetGainsForStudentIdQueryHandler(ICommonService commonService,
                                              IQuestionDal questionDal) : IRequestHandler<GetStudentGainsForSelfQuery, GetStudentGainsModel>
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
                            && x.CreateDate.Date >= startDate.Date
                            && x.CreateDate.Date <= endDate.Date.AddDays(1).AddSeconds(-1),
            include: x => x.Include(u => u.Lesson).Include(u => u.Gain),
            selector: x => new { Lesson = x.Lesson != null ? x.Lesson.Name : string.Empty, Gain = x.Gain != null ? x.Gain.Name : string.Empty, x.CreateDate },
            cancellationToken: cancellationToken);

        var allQuestions = questions.ToList();

        result.ForLessons = allQuestions.Distinct()
            .GroupBy(x => x.Lesson)
            .Select(g => new { Lesson = g.Key, Count = g.Count() })
            .ToDictionary(x => x.Lesson!, x => x.Count);

        result.ForGains = allQuestions
            .Where(x => x.Gain.IsNotEmpty())
            .GroupBy(x => x.Gain)
            .Select(g => new { Gain = g.Key, Count = g.Count() })
            .ToDictionary(x => x.Gain!, x => x.Count);

        result.ForLessonGains = allQuestions
            .Where(x => x.Gain.IsNotEmpty())
            .GroupBy(x => x.Lesson)
            .Select(g => new
            {
                Lesson = g.Key,
                Gains = g.GroupBy(y => y.Gain)
                         .Select(y => new { Gain = y.Key, Count = y.Count() })
                         .ToDictionary(y => y.Gain!, y => y.Count)
            })
            .ToDictionary(x => x.Lesson!, x => x.Gains);

        result.SendQuestionByDay = allQuestions
            .GroupBy(x => x.CreateDate.ToStringDayOfWeek())
            .Select(x => new { Day = x.Key, Count = x.Count() })
            .ToDictionary(x => x.Day, x => x.Count);

        result.Info = new Dictionary<string, int>
        {
            { "TotalQuestion", allQuestions.Count },
            { "TotalGain", result.ForGains.Distinct().Sum(x=>x.Value) }
        };

        return result;
    }
}