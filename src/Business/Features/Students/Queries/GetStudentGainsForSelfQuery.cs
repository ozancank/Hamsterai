using Business.Features.Students.Models;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Students.Queries;

public class GetStudentGainsForSelfQuery : IRequest<GetStudenGainsModel>, ISecuredRequest<UserTypes>
{
    public UserTypes[] Roles { get; } = [UserTypes.Student];
}

public class GetGainsForStudentIdQueryHandler(ICommonService commonService,
                                              IQuestionDal questionDal,
                                              IQuizQuestionDal quizQuestionDal,
                                              ISimilarQuestionDal similarQuestionDal) : IRequestHandler<GetStudentGainsForSelfQuery, GetStudenGainsModel>
{
    public async Task<GetStudenGainsModel> Handle(GetStudentGainsForSelfQuery request, CancellationToken cancellationToken)
    {
        var result = new GetStudenGainsModel();
        var userId = commonService.HttpUserId;

        var questions = await questionDal.GetListAsync(
            enableTracking: false,
            predicate: x => x.CreateUser == userId && x.Status == QuestionStatus.Answered && x.GainId.HasValue,
            include: x => x.Include(u => u.Lesson).Include(u => u.Gain),
            selector: x => new { Lesson = x.Lesson.Name, Gain = x.Gain.Name },
            cancellationToken: cancellationToken);

        var similarQuestions = await similarQuestionDal.GetListAsync(
            enableTracking: false,
            predicate: x => x.CreateUser == userId && x.Status == QuestionStatus.Answered && x.GainId.HasValue,
            include: x => x.Include(u => u.Lesson).Include(u => u.Gain),
            selector: x => new { Lesson = x.Lesson.Name, Gain = x.Gain.Name },
            cancellationToken: cancellationToken);

        var quizQuestions = await quizQuestionDal.GetListAsync(
            enableTracking: false,
            predicate: x => x.CreateUser == userId && x.Quiz.Status == QuizStatus.Completed,
            include: x => x.Include(x => x.Quiz).ThenInclude(u => u.Lesson)
                           .Include(u => u.Gain),
            selector: x => new { Lesson = x.Quiz.Lesson.Name, Gain = x.Gain.Name },
            cancellationToken: cancellationToken);

        var allQuestions = questions.Concat(similarQuestions).Concat(quizQuestions).ToList();

        result.ForLessons = allQuestions.Distinct()
            .GroupBy(x => x.Lesson)
            .Select(g => new { Lesson = g.Key, Count = g.Count() })
            .ToDictionary(x => x.Lesson, x => x.Count);

        result.ForGains = allQuestions
            .GroupBy(x => x.Gain)
            .Select(g => new { Gain = g.Key, Count = g.Count() })
            .ToDictionary(x => x.Gain, x => x.Count);

        result.ForLessonGains = allQuestions
            .GroupBy(x => x.Lesson)
            .Select(g => new
            {
                Lesson = g.Key,
                Gains = g.GroupBy(y => y.Gain)
                         .Select(y => new { Gain = y.Key, Count = y.Count() })
                         .ToDictionary(y => y.Gain, y => y.Count)
            })
            .ToDictionary(x => x.Lesson, x => x.Gains);

        return result;
    }
}