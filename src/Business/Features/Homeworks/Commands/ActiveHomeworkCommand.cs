using Business.Features.Homeworks.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Homeworks.Commands;

public class ActiveHomeworkCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required string HomeworkId { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Teacher];
    public string[] HidePropertyNames { get; } = [];
}

public class ActiveHomeworkCommandHandler(IHomeworkDal homeworkDal,
                                           IHomeworkStudentDal homeworkStudentDal,
                                           ICommonService commonService) : IRequestHandler<ActiveHomeworkCommand, bool>
{
    public async Task<bool> Handle(ActiveHomeworkCommand request, CancellationToken cancellationToken)
    {
        var userId = commonService.HttpUserId;
        var date = DateTime.Now;

        var homework = await homeworkDal.GetAsync(predicate: x => x.Id == request.HomeworkId, cancellationToken: cancellationToken);
        await HomeworkRules.HomeworkShouldExists(homework);

        var homeworkStudents = await homeworkStudentDal.GetListAsync(predicate: x => x.HomeworkId == request.HomeworkId, cancellationToken: cancellationToken);

        homework.UpdateUser = userId;
        homework.UpdateDate = date;
        homework.IsActive = true;

        foreach (var item in homeworkStudents)
        {
            item.UpdateUser = userId;
            item.UpdateDate = date;
            item.IsActive = true;
        }

        await homeworkDal.ExecuteWithTransactionAsync(async () =>
        {
            await homeworkStudentDal.UpdateRangeAsync(homeworkStudents, cancellationToken: cancellationToken);
            await homeworkDal.UpdateAsync(homework, cancellationToken: cancellationToken);
        }, cancellationToken: cancellationToken);

        return true;
    }
}