﻿using Application.Features.Students.Rules;
using Application.Features.Users.Rules;
using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Caching;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Students.Commands;

public class PassiveStudentCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest, ICacheRemoverRequest
{
    public int Id { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
    public string[] CacheKey { get; } = [$"^{Strings.CacheStatusAndLicence}"];
}

public class PassiveStudentCommandHandler(IStudentDal studentDal,
                                          IUserDal userDal,
                                          ICommonService commonService) : IRequestHandler<PassiveStudentCommand, bool>
{
    public async Task<bool> Handle(PassiveStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await studentDal.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        await StudentRules.StudentShouldExists(student);

        var user = await userDal.GetAsync(x => x.ConnectionId == student.Id && x.Type == UserTypes.Student, cancellationToken: cancellationToken);
        await UserRules.UserShouldExists(user);

        student.UpdateUser = commonService.HttpUserId;
        student.UpdateDate = DateTime.Now;
        student.IsActive = false;

        user.IsActive = false;

        await studentDal.ExecuteWithTransactionAsync(async () =>
        {
            await studentDal.UpdateAsync(student, cancellationToken: cancellationToken);
            await userDal.UpdateAsync(user, cancellationToken: cancellationToken);
        }, cancellationToken: cancellationToken);

        return true;
    }
}