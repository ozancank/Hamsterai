using Application.Features.Schools.Rules;
using Application.Features.Teachers.Models;
using Application.Features.Teachers.Rules;
using Application.Features.Users.Rules;
using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using Domain.Entities.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Caching;
using OCK.Core.Pipelines.Logging;
using OCK.Core.Security.HashingHelper;

namespace Application.Features.Teachers.Commands;

public class AddTeacherCommand : IRequest<GetTeacherModel>, ISecuredRequest<UserTypes>, ILoggableRequest, ICacheRemoverRequest
{
    public required AddTeacherModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.School];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
    public string[] CacheKey { get; } = [$"^{Strings.CacheStatusAndLicence}"];
}

public class AddTeacherCommandHandler(IMapper mapper,
                                      ITeacherDal teacherDal,
                                      ICommonService commonService,
                                      IUserDal userDal,
                                      ISchoolDal schoolDal,
                                      UserRules userRules,
                                      TeacherRules teacherRules) : IRequestHandler<AddTeacherCommand, GetTeacherModel>
{
    public async Task<GetTeacherModel> Handle(AddTeacherCommand request, CancellationToken cancellationToken)
    {
        request.Model.Email = request.Model.Email!.Trim().ToLower();

        await teacherRules.TeacherEmailCanNotBeDuplicated(request.Model.Email);

        await userRules.UserNameCanNotBeDuplicated(request.Model.Email);
        await userRules.UserEmailCanNotBeDuplicated(request.Model.Email);

        var userId = commonService.HttpUserId;
        var schoolId = commonService.HttpSchoolId;
        var date = DateTime.Now;

        var school = await schoolDal.GetAsync(
            enableTracking: false,
            predicate: x => x.Id == schoolId,
            selector: x => new { x.Id, x.LicenseEndDate },
            cancellationToken: cancellationToken);
        await SchoolRules.SchoolShouldExists(school);

        var teacher = mapper.Map<Teacher>(request.Model);
        teacher.Id = await teacherDal.GetNextPrimaryKeyAsync(x => x.Id, cancellationToken: cancellationToken);
        teacher.IsActive = true;
        teacher.CreateUser = userId;
        teacher.CreateDate = date;
        teacher.UpdateUser = userId;
        teacher.UpdateDate = date;
        teacher.SchoolId = commonService.HttpSchoolId ?? 0;

        HashingHelper.CreatePasswordHash(AppOptions.DefaultPassword, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new User
        {
            Id = await userDal.GetNextPrimaryKeyAsync(x => x.Id, cancellationToken: cancellationToken),
            CreateDate = date,
            UserName = teacher.Email!.Trim().ToLower(),
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            MustPasswordChange = true,
            IsActive = true,
            Name = teacher.Name,
            Surname = string.Empty,
            Phone = teacher.Phone?.TrimForPhone(),
            ProfileUrl = string.Empty,
            Email = teacher.Email.Trim(),
            Type = UserTypes.Teacher,
            SchoolId = commonService.HttpSchoolId,
            ConnectionId = teacher.Id,
            LicenceEndDate = school.LicenseEndDate,
        };

        var result = await teacherDal.ExecuteWithTransactionAsync(async () =>
        {
            var added = await teacherDal.AddAsyncCallback(teacher, cancellationToken: cancellationToken);
            await userDal.AddAsync(user, cancellationToken: cancellationToken);
            var result = mapper.Map<GetTeacherModel>(added);
            return result;
        }, cancellationToken: cancellationToken);

        return result;
    }
}

public class AddTeacherCommandValidator : AbstractValidator<AddTeacherCommand>
{
    public AddTeacherCommandValidator()
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