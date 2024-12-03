using Application.Features.Schools.Models.ClassRooms;
using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Schools.Queries.ClassRooms;

public class GetClassRoomsGainsByIdQuery : IRequest<GetClassRoomGainsModel>, ISecuredRequest<UserTypes>
{
    public required ClassRoomGainsRequestModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.School];
    public bool AllowByPass => false;
}

public class GetClassRoomsGainsByIdQueryHandler(ICommonService commonService,
                                                IQuestionDal questionDal,
                                                IClassRoomDal classRoomDal,
                                                IUserDal userDal) : IRequestHandler<GetClassRoomsGainsByIdQuery, GetClassRoomGainsModel>
{
    public async Task<GetClassRoomGainsModel> Handle(GetClassRoomsGainsByIdQuery request, CancellationToken cancellationToken)
    {
        var result = new GetClassRoomGainsModel();
        var userType = commonService.HttpUserType;
        var schoolId = commonService.HttpSchoolId;

        var studentIds = (await classRoomDal.GetAsync(
            enableTracking: false,
            predicate: userType == UserTypes.Administator
                ? x => x.Id == request.Model.ClassRoomId
                : x => x.Id == request.Model.ClassRoomId && x.SchoolId == schoolId,
            selector: x => x.Students.Select(x => x.Id),
            cancellationToken: cancellationToken)).ToList();

        var userIds = (await userDal.GetListAsync(
            enableTracking: false,
            predicate: x => x.Type == UserTypes.Student
                            && x.ConnectionId != null
                            && x.ConnectionId > 0
                            && studentIds.Contains(x.ConnectionId.Value)
                            && (userType == UserTypes.Administator || x.SchoolId == schoolId),
            selector: x => x.Id,
            cancellationToken: cancellationToken)).ToList();

        if (request.Model.StartDate == null) request.Model.StartDate = DateTime.Today.AddMonths(-1);
        if (request.Model.EndDate == null) request.Model.EndDate = DateTime.Today.AddDays(1).AddMilliseconds(-1);

        var questions = await questionDal.GetListAsync(
            enableTracking: false,
            predicate: x => userIds.Contains(x.CreateUser)
                            && x.Status == QuestionStatus.Answered
                            && x.GainId.HasValue
                            && x.CreateDate.Date >= request.Model.StartDate.Value.Date
                            && x.CreateDate.Date <= request.Model.EndDate.Value.Date.AddDays(1).AddMilliseconds(-1),
            include: x => x.Include(u => u.Lesson).Include(u => u.Gain),
            selector: x => new { Lesson = x.Lesson!.Name, Gain = x.Gain!.Name },
            cancellationToken: cancellationToken);

        var allQuestions = questions.ToList();

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