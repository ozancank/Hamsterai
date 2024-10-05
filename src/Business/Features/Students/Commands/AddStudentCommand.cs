using Business.Features.Students.Models;
using Business.Features.Students.Rules;
using Business.Features.Users.Rules;
using Business.Services.CommonService;
using DataAccess.Abstract.Core;
using Domain.Entities.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;
using OCK.Core.Security.HashingHelper;

namespace Business.Features.Students.Commands;

public class AddStudentCommand : IRequest<GetStudentModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public AddStudentModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.School];
    public string[] HidePropertyNames { get; } = [];
}

public class AddStudentCommandHandler(IMapper mapper,
                                      IStudentDal studentDal,
                                      ICommonService commonService,
                                      IUserDal userDal,
                                      UserRules userRules,
                                      StudentRules studentRules) : IRequestHandler<AddStudentCommand, GetStudentModel>
{
    public async Task<GetStudentModel> Handle(AddStudentCommand request, CancellationToken cancellationToken)
    {
        await studentRules.StudentTcNoCanNotBeDuplicated(request.Model.TcNo);
        await studentRules.StudentEmailCanNotBeDuplicated(request.Model.Email);
        await studentRules.StudentPhoneCanNotBeDuplicated(request.Model.Phone);
        await userRules.UserNameCanNotBeDuplicated(request.Model.Email);
        await userRules.UserEmailCanNotBeDuplicated(request.Model.Email);
        await userRules.UserPhoneCanNotBeDuplicated(request.Model.Phone);

        var userId = commonService.HttpUserId;
        var date = DateTime.Now;

        var student = mapper.Map<Student>(request.Model);
        student.Id = await studentDal.GetNextPrimaryKeyAsync(x => x.Id, cancellationToken: cancellationToken);
        student.IsActive = true;
        student.CreateUser = userId;
        student.CreateDate = date;
        student.UpdateUser = userId;
        student.UpdateDate = date;

        HashingHelper.CreatePasswordHash(AppOptions.DefaultPassword, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new User
        {
            Id = await userDal.GetNextPrimaryKeyAsync(x => x.Id, cancellationToken: cancellationToken),
            CreateDate = date,
            UserName = student.Email.Trim().ToLower(),
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            MustPasswordChange = true,
            IsActive = true,
            Name = student.Name,
            Surname = string.Empty,
            Phone = student.Phone?.TrimForPhone(),
            ProfileUrl = string.Empty,
            Email = student.Email.Trim(),
            Type = UserTypes.Student,
            SchoolId = commonService.HttpSchoolId,
            ConnectionId = student.Id,
        };

        var result = await studentDal.ExecuteWithTransactionAsync(async () =>
        {
            var added = await studentDal.AddAsyncCallback(student, cancellationToken: cancellationToken);
            await userDal.AddAsync(user, cancellationToken: cancellationToken);
            var result = mapper.Map<GetStudentModel>(added);
            return result;
        }, cancellationToken: cancellationToken);

        return result;
    }
}

public class AddStudentCommandValidator : AbstractValidator<AddStudentCommand>
{
    public AddStudentCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotNull().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.Name).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Model.Name).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Model.Name).MaximumLength(250).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "250"]);

        RuleFor(x => x.Model.Surname).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Surname]);
        RuleFor(x => x.Model.Surname).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Surname, "2"]);
        RuleFor(x => x.Model.Surname).MaximumLength(250).WithMessage(Strings.DynamicMaxLength, [Strings.Surname, "100"]);

        RuleFor(x => x.Model.StudentNo).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [$"{Strings.Student} {Strings.No}"]);
        RuleFor(x => x.Model.StudentNo).MinimumLength(1).WithMessage(Strings.DynamicMinLength, [$"{Strings.Student} {Strings.No}", "1"]);
        RuleFor(x => x.Model.StudentNo).MaximumLength(15).WithMessage(Strings.DynamicMaxLength, [$"{Strings.Student} {Strings.No}", "15"]);

        RuleFor(x => x.Model.TcNo).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [$"{Strings.Identity} {Strings.No}"]);
        RuleFor(x => x.Model.TcNo).Length(11).WithMessage(Strings.DynamicLength, [$"{Strings.Identity} {Strings.No}", "11"]);
        RuleFor(x => x.Model.TcNo).Must(x => double.TryParse(x, out _)).WithMessage(Strings.DynamicOnlyDigit, [$"{Strings.Identity} {Strings.No}"]);

        RuleFor(x => x.Model.Email).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [$"{Strings.Authorized} {Strings.OfEmail}"]);
        RuleFor(x => x.Model.Email).MinimumLength(5).WithMessage(Strings.DynamicMinLength, [$"{Strings.Authorized} {Strings.OfEmail}", "5"]);
        RuleFor(x => x.Model.Email).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [$"{Strings.Authorized} {Strings.OfEmail}", "100"]);
        RuleFor(x => x.Model.Email).EmailAddress().WithMessage(Strings.EmailWrongFormat);

        RuleFor(x => x.Model.Phone).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [$"{Strings.Authorized} {Strings.OfPhone}"]);
        RuleFor(x => x.Model.Phone).MinimumLength(10).WithMessage(Strings.DynamicMinLength, [$"{Strings.Authorized} {Strings.OfPhone}", "10"]);
        RuleFor(x => x.Model.Phone).MaximumLength(15).WithMessage(Strings.DynamicMaxLength, [$"{Strings.Authorized} {Strings.OfPhone}", "15"]);
        RuleFor(x => x.Model.Phone).Must(x => double.TryParse(x, out _)).WithMessage(Strings.DynamicOnlyDigit, [$"{Strings.Authorized} {Strings.OfPhone}"]);
    }
}