using Application.Features.Homeworks.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Homeworks.Commands;

public class UpdateHomeworkReadCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required string HomeworkStudentId { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.Student, UserTypes.Person];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class UpdateHomeworkReadCommandHandler(ICommonService commonService,
                                              IHomeworkStudentDal homeworkStudentDal,
                                              IHomeworkUserDal homeworkUserDal) : IRequestHandler<UpdateHomeworkReadCommand, bool>
{
    public async Task<bool> Handle(UpdateHomeworkReadCommand request, CancellationToken cancellationToken)
    {
        var userId = commonService.HttpUserId;
        var userType = commonService.HttpUserType;
        var connectionId = commonService.HttpConnectionId;
        var date = DateTime.Now;

        if (userType == UserTypes.Student)
        {
            var homework = await homeworkStudentDal.GetAsync(
                predicate: x => x.Id == request.HomeworkStudentId && x.StudentId == connectionId,
                cancellationToken: cancellationToken);

            await HomeworkRules.HomeworkStudentShouldExists(homework);

            homework.UpdateUser = userId;
            homework.UpdateDate = date;
            homework.Status = HomeworkStatus.Seen;

            await homeworkStudentDal.UpdateAsync(homework, cancellationToken: cancellationToken);
        }
        else
        {
            var homework = await homeworkUserDal.GetAsync(
                predicate: x => x.Id == request.HomeworkStudentId && x.UserId == userId,
                cancellationToken: cancellationToken);
            await HomeworkRules.HomeworkUserShouldExists(homework);
            homework.UpdateUser = userId;
            homework.UpdateDate = date;
            homework.Status = HomeworkStatus.Seen;
            await homeworkUserDal.UpdateAsync(homework, cancellationToken: cancellationToken);
        }

        return true;
    }
}

public class UpdateHomeworkReadCommandValidator : AbstractValidator<UpdateHomeworkReadCommand>
{
    public UpdateHomeworkReadCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.HomeworkStudentId).NotEmpty().WithMessage(Strings.InvalidValue);
    }
}