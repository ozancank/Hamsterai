using Application.Features.Homeworks.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Homeworks.Commands;

public class PassiveHomeworkCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required string HomeworkId { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.Teacher];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class PassiveHomeworkCommandHandler(IHomeworkDal homeworkDal,
                                           IHomeworkStudentDal homeworkStudentDal,
                                           IHomeworkUserDal homeworkUserDal,
                                           ICommonService commonService) : IRequestHandler<PassiveHomeworkCommand, bool>
{
    public async Task<bool> Handle(PassiveHomeworkCommand request, CancellationToken cancellationToken)
    {
        var userId = commonService.HttpUserId;
        var date = DateTime.Now;

        var homework = await homeworkDal.GetAsync(predicate: x => x.Id == request.HomeworkId, cancellationToken: cancellationToken);
        await HomeworkRules.HomeworkShouldExists(homework);

        var homeworkStudents = await homeworkStudentDal.GetListAsync(predicate: x => x.HomeworkId == request.HomeworkId, cancellationToken: cancellationToken);
        var homeworkUsers = await homeworkUserDal.GetListAsync(predicate: x => x.HomeworkId == request.HomeworkId, cancellationToken: cancellationToken);

        homework.UpdateUser = userId;
        homework.UpdateDate = date;
        homework.IsActive = false;

        foreach (var item in homeworkStudents)
        {
            item.UpdateUser = userId;
            item.UpdateDate = date;
            item.IsActive = false;
        }

        foreach (var item in homeworkUsers)
        {
            item.UpdateUser = userId;
            item.UpdateDate = date;
            item.IsActive = false;
        }

        await homeworkDal.ExecuteWithTransactionAsync(async () =>
        {
            if (homeworkUsers.Count > 0) await homeworkUserDal.UpdateRangeAsync(homeworkUsers, cancellationToken: cancellationToken);
            if (homeworkStudents.Count > 0) await homeworkStudentDal.UpdateRangeAsync(homeworkStudents, cancellationToken: cancellationToken);
            await homeworkDal.UpdateAsync(homework, cancellationToken: cancellationToken);
        }, cancellationToken: cancellationToken);

        return true;
    }
}