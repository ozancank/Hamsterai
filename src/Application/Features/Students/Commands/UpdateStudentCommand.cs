using Application.Features.Schools.Rules;
using Application.Features.Students.Models;
using Application.Features.Students.Rules;
using Application.Features.Users.Rules;
using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Caching;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Students.Commands;

public class UpdateStudentCommand : IRequest<GetStudentModel>, ISecuredRequest<UserTypes>, ILoggableRequest, ICacheRemoverRequest
{
    public required UpdateStudentModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.School];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
    public string[] CacheKey { get; } = [$"^{Strings.CacheStatusAndLicence}"];
}

public class UpdateStudentCommandHandler(IMapper mapper,
                                         IStudentDal studentDal,
                                         ICommonService commonService,
                                         ISchoolDal schoolDal,
                                         IUserDal userDal,
                                         UserRules userRules,
                                         StudentRules studentRules,
                                         IClassRoomDal classRoomDal) : IRequestHandler<UpdateStudentCommand, GetStudentModel>
{
    public async Task<GetStudentModel> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await studentDal.GetAsync(x => x.Id == request.Model.Id, cancellationToken: cancellationToken);

        await StudentRules.StudentShouldExists(student);
        await studentRules.StudentEmailCanNotBeDuplicated(request.Model.Email!, request.Model.Id);
        await studentRules.StudentPhoneCanNotBeDuplicated(request.Model.Phone!, request.Model.Id);

        var user = await userDal.GetAsync(x => x.ConnectionId == student.Id && x.Type == UserTypes.Student, cancellationToken: cancellationToken);

        await UserRules.UserShouldExists(user);
        await userRules.UserNameCanNotBeDuplicated(request.Model.Email!, user.Id);
        await userRules.UserEmailCanNotBeDuplicated(request.Model.Email!, user.Id);
        await userRules.UserPhoneCanNotBeDuplicated(request.Model.Phone!, user.Id);

        var classRoom = await classRoomDal.GetAsync(
            enableTracking: false,
            predicate: x => x.Id == request.Model.ClassRoomId && x.SchoolId == commonService.HttpSchoolId,
            cancellationToken: cancellationToken);
        await ClassRoomRules.ClassRoomShouldExistsAndActive(classRoom);

        var userId = commonService.HttpUserId;
        var schoolId = commonService.HttpSchoolId;
        var date = DateTime.Now;

        var school = await schoolDal.GetAsync(
            enableTracking: false,
            predicate: x => x.Id == schoolId,
            selector: x => new { x.Id, x.LicenseEndDate },
            cancellationToken: cancellationToken);
        await SchoolRules.SchoolShouldExists(school);

        student.UpdateUser = userId;
        student.UpdateDate = date;
        student.Name = request.Model.Name;
        student.Surname = request.Model.Surname;
        student.StudentNo = request.Model.StudentNo;
        student.Email = request.Model.Email!.Trim().ToLower();
        student.Phone = request.Model.Phone.TrimForPhone();
        student.ClassRoomId = request.Model.ClassRoomId;

        user.Name = student.Name;
        user.Surname = student.Surname;
        user.UserName = student.Email;
        user.Email = student.Email;
        user.Phone = student.Phone;
        user.LicenceEndDate = school.LicenseEndDate;
        //user.GroupId = classRoom.PackageId;

        var result = await studentDal.ExecuteWithTransactionAsync(async () =>
        {
            var added = await studentDal.UpdateAsyncCallback(student, cancellationToken: cancellationToken);
            await userDal.UpdateAsync(user, cancellationToken: cancellationToken);
            var result = mapper.Map<GetStudentModel>(added);
            return result;
        }, cancellationToken: cancellationToken);

        return result;
    }
}

public class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
{
    public UpdateStudentCommandValidator()
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
        RuleFor(x => x.Model.StudentNo).MaximumLength(15).WithMessage(Strings.DynamicMinLength, [$"{Strings.Student} {Strings.No}", "15"]);

        //RuleFor(x => x.Model.TcNo).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [$"{Strings.Identity} {Strings.No}"]);
        //RuleFor(x => x.Model.TcNo).Length(11).WithMessage(Strings.DynamicLength, [$"{Strings.Identity} {Strings.No}", "11"]);
        //RuleFor(x => x.Model.TcNo).Must(x => double.TryParse(x, out _)).WithMessage(Strings.DynamicOnlyDigit, [$"{Strings.Identity} {Strings.No}"]);
        RuleFor(x => x.Model.TcNo).MaximumLength(11).WithMessage(Strings.DynamicMaxLength, [$"{Strings.Identity} {Strings.No}", "11"]);

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