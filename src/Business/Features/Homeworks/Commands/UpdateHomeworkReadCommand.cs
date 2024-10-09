using Business.Features.Homeworks.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Homeworks.Commands;

public class UpdateHomeworkReadCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public string HomeworkStudentId { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Student];
    public string[] HidePropertyNames { get; } = [];
}

public class UpdateHomeworkReadCommandHandler(ICommonService commonService,
                                              IHomeworkStudentDal homeworkStudentDal) : IRequestHandler<UpdateHomeworkReadCommand, bool>
{
    public async Task<bool> Handle(UpdateHomeworkReadCommand request, CancellationToken cancellationToken)
    {
        var userId = commonService.HttpUserId;
        var connectionId = commonService.HttpConnectionId;
        var date = DateTime.Now;

        var homework = await homeworkStudentDal.GetAsync(
            predicate: x => x.Id == request.HomeworkStudentId && x.StudentId == connectionId,
            cancellationToken: cancellationToken);

        await HomeworkRules.HomeworkStudentShouldExists(homework);

        homework.UpdateUser = userId;
        homework.UpdateDate = date;
        homework.Status = HomeworkStatus.Seen;

        await homeworkStudentDal.UpdateAsync(homework, cancellationToken: cancellationToken);

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