using Application.Features.Schools.Rules;
using Application.Features.Teachers.Models;
using Application.Features.Teachers.Rules;
using Application.Features.Users.Rules;
using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Caching;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Teachers.Commands;

public class UpdateTeacherCommand : IRequest<GetTeacherModel>, ISecuredRequest<UserTypes>, ILoggableRequest, ICacheRemoverRequest
{
    public required UpdateTeacherModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.School];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
    public string[] CacheKey { get; } = [$"^{Strings.CacheStatusAndLicence}"];
}

public class UpdateTeacherCommandHandler(IMapper mapper,
                                         ITeacherDal teacherDal,
                                         ICommonService commonService,
                                         IUserDal userDal,
                                         ISchoolDal schoolDal,
                                         UserRules userRules,
                                         TeacherRules teacherRules) : IRequestHandler<UpdateTeacherCommand, GetTeacherModel>
{
    public async Task<GetTeacherModel> Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
    {
        request.Model.Email = request.Model.Email!.Trim().ToLower();

        var teacher = await teacherDal.GetAsync(x => x.Id == request.Model.Id, cancellationToken: cancellationToken);

        await TeacherRules.TeacherShouldExists(teacher);
        await teacherRules.TeacherEmailCanNotBeDuplicated(request.Model.Email, request.Model.Id);

        var user = await userDal.GetAsync(x => x.ConnectionId == teacher.Id && x.Type == UserTypes.Teacher, cancellationToken: cancellationToken);

        await UserRules.UserShouldExists(user);
        await userRules.UserNameCanNotBeDuplicated(request.Model.Email, user.Id);
        await userRules.UserEmailCanNotBeDuplicated(request.Model.Email, user.Id);

        var userId = commonService.HttpUserId;
        var schoolId = commonService.HttpSchoolId;
        var date = DateTime.Now;

        var school = await schoolDal.GetAsync(
            enableTracking: false,
            predicate: x => x.Id == schoolId,
            selector: x => new { x.Id, x.LicenseEndDate },
            cancellationToken: cancellationToken);
        await SchoolRules.SchoolShouldExists(school);

        teacher.Name = request.Model.Name;
        teacher.Surname = request.Model.Surname;
        teacher.Email = request.Model.Email;
        teacher.Phone = request.Model.Phone.TrimForPhone();
        teacher.Branch = request.Model.Branch;

        user.Name = teacher.Name;
        user.UserName = teacher.Email!.Trim().ToLower();
        user.Email = teacher.Email.Trim().ToLower();
        user.Phone = teacher.Phone.TrimForPhone();
        user.LicenceEndDate = school.LicenseEndDate;

        var result = await teacherDal.ExecuteWithTransactionAsync(async () =>
        {
            var added = await teacherDal.UpdateAsyncCallback(teacher, cancellationToken: cancellationToken);
            await userDal.UpdateAsync(user, cancellationToken: cancellationToken);
            var result = mapper.Map<GetTeacherModel>(added);
            return result;
        }, cancellationToken: cancellationToken);

        return result;
    }
}

public class UpdateTeacherCommandValidator : AbstractValidator<UpdateTeacherCommand>
{
    public UpdateTeacherCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotNull().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.Name).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Model.Name).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Model.Name).MaximumLength(250).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "250"]);

        RuleFor(x => x.Model.Surname).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Surname]);
        RuleFor(x => x.Model.Surname).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Surname, "2"]);
        RuleFor(x => x.Model.Surname).MaximumLength(250).WithMessage(Strings.DynamicMaxLength, [Strings.Surname, "100"]);

        RuleFor(x => x.Model.Email).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [$"{Strings.Authorized} {Strings.OfEmail}"]);
        RuleFor(x => x.Model.Email).MinimumLength(5).WithMessage(Strings.DynamicMinLength, [$"{Strings.Authorized} {Strings.OfEmail}", "5"]);
        RuleFor(x => x.Model.Email).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [$"{Strings.Authorized} {Strings.OfEmail}", "100"]);
        RuleFor(x => x.Model.Email).EmailAddress().WithMessage(Strings.EmailWrongFormat);

        //RuleFor(x => x.Model.Phone).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [$"{Strings.Authorized} {Strings.OfPhone}"]);
        //RuleFor(x => x.Model.Phone).MinimumLength(10).WithMessage(Strings.DynamicMinLength, [$"{Strings.Authorized} {Strings.OfPhone}", "10"]);
        RuleFor(x => x.Model.Phone).MaximumLength(15).WithMessage(Strings.DynamicMaxLength, [$"{Strings.Authorized} {Strings.OfPhone}", "15"]);
        //RuleFor(x => x.Model.Phone).Must(x => double.TryParse(x, out _)).WithMessage(Strings.DynamicOnlyDigit, [$"{Strings.Authorized} {Strings.OfPhone}"]);

        RuleFor(x => x.Model.Branch).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Branch]);
        RuleFor(x => x.Model.Branch).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Branch, "2"]);
        RuleFor(x => x.Model.Branch).MaximumLength(50).WithMessage(Strings.DynamicMaxLength, [Strings.Branch, "50"]);
    }
}