using Application.Features.Students.Models;
using Application.Features.Users.Rules;
using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Students.Queries;

public class GetStudentGainsByIdQuery : IRequest<GetStudentGainsModel>, ISecuredRequest<UserTypes>
{
    public required StudentGainsRequestModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.School, UserTypes.Teacher, UserTypes.Person];
    public bool AllowByPass => false;
}

public class GetStudentGainsByIdQueryHandler(ICommonService commonService,
                                             IQuestionDal questionDal,
                                             IUserDal userDal) : IRequestHandler<GetStudentGainsByIdQuery, GetStudentGainsModel>
{
    public async Task<GetStudentGainsModel> Handle(GetStudentGainsByIdQuery request, CancellationToken cancellationToken)
    {
        var result = new GetStudentGainsModel();
        var userType = commonService.HttpUserType;

        var user = await userDal.GetAsync(
            enableTracking: false,
            predicate: userType == UserTypes.Administator
                ? x => x.Id == request.Model.UserId
                : userType == UserTypes.Person
                    ? x => x.Type == UserTypes.Person && x.Id == commonService.HttpUserId
                    : x => x.Type == UserTypes.Student && (x.ConnectionId == request.Model.StudentId || x.Id == request.Model.UserId) && x.SchoolId == commonService.HttpSchoolId,
            cancellationToken: cancellationToken);
        await UserRules.UserShouldExistsAndActive(user);

        var userId = user.Id;

        if (request.Model.StartDate == null) request.Model.StartDate = DateTime.Today.AddMonths(-1);
        if (request.Model.EndDate == null) request.Model.EndDate = DateTime.Today.AddDays(1).AddMilliseconds(-1);

        var allQuestions = await questionDal.GetListAsync(
            enableTracking: false,
            predicate: x => x.CreateUser == userId
                            && x.Status == QuestionStatus.Answered
                            && x.CreateDate.Date >= request.Model.StartDate.Value.Date
                            && x.CreateDate.Date <= request.Model.EndDate.Value.Date.AddDays(1).AddMilliseconds(-1),
            include: x => x.Include(u => u.Lesson).Include(u => u.Gain),
            selector: x => new { Lesson = x.Lesson!.Name, Gain = x.Gain!.Name, x.CreateDate },
            cancellationToken: cancellationToken);

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