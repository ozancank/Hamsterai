using Business.Features.Lessons.Rules;
using Business.Features.Schools.Models.Schools;
using Business.Features.Schools.Rules;
using Business.Features.Users.Rules;
using Business.Services.CommonService;
using DataAccess.Abstract.Core;
using Domain.Entities.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Schools.Commands.Schools;

public class UpdateSchoolCommand : IRequest<GetSchoolModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public UpdateSchoolModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public string[] HidePropertyNames { get; } = [];
}

public class UpdateSchoolCommandHandler(IMapper mapper,
                                        ISchoolDal schoolDal,
                                        ICommonService commonService,
                                        IUserDal userDal,
                                        ISchoolGroupDal schoolGroupDal,
                                        UserRules userRules,
                                        SchoolRules schoolRules,
                                        GroupRules groupRules) : IRequestHandler<UpdateSchoolCommand, GetSchoolModel>
{
    public async Task<GetSchoolModel> Handle(UpdateSchoolCommand request, CancellationToken cancellationToken)
    {
        var school = await schoolDal.GetAsync(x => x.Id == request.Model.Id, cancellationToken: cancellationToken);

        await SchoolRules.SchoolShouldExists(school);
        await schoolRules.SchoolNameAndCityCanNotBeDuplicated(request.Model.Name, request.Model.City, school.Id);
        await schoolRules.SchoolTaxNumberCanNotBeDuplicated(request.Model.TaxNumber, school.Id);
        await groupRules.GroupShouldBeRecordInDatabase(request.Model.GroupIds);

        var user = await userDal.GetAsync(x => x.SchoolId == school.Id && x.Type == UserTypes.School, cancellationToken: cancellationToken);

        await UserRules.UserShouldExists(user);
        await userRules.UserNameCanNotBeDuplicated(request.Model.AuthorizedEmail, user.Id);
        await userRules.UserEmailCanNotBeDuplicated(request.Model.AuthorizedEmail, user.Id);
        await userRules.UserPhoneCanNotBeDuplicated(request.Model.AuthorizedPhone, user.Id);

        var userId = commonService.HttpUserId;
        var date = DateTime.Now;

        school.Name = request.Model.Name;
        school.TaxNumber = request.Model.TaxNumber;
        school.Address = request.Model.Address;
        school.City = request.Model.City;
        school.AuthorizedName = request.Model.AuthorizedName;
        school.AuthorizedPhone = request.Model.AuthorizedPhone;
        school.AuthorizedEmail = request.Model.AuthorizedEmail;
        school.LicenseEndDate = request.Model.LicenseEndDate;
        school.UserCount = request.Model.UserCount;

        user.Name = school.Name;
        user.UserName = school.AuthorizedEmail.Trim().ToLower();
        user.Email = school.AuthorizedEmail.Trim().ToLower();
        user.Phone = school.AuthorizedPhone.TrimForPhone();

        var deleteList = await schoolGroupDal.GetListAsync(predicate: x => x.SchoolId == school.Id, cancellationToken: cancellationToken);

        var schoolGroups = request.Model.GroupIds.Select(x => new SchoolGroup
        {
            Id = Guid.NewGuid(),
            IsActive = true,
            CreateUser = userId,
            CreateDate = date,
            UpdateUser = userId,
            UpdateDate = date,
            SchoolId = school.Id,
            GroupId = x,
        }).ToList();

        var updateList = new List<User>();
        var usersInSchool = await userDal.GetListAsync(predicate: x => x.SchoolId == school.Id && x.Type == UserTypes.Student, cancellationToken: cancellationToken);
        foreach (var student in usersInSchool)
        {
            if (student.GroupId != null && !request.Model.GroupIds.Contains(student.GroupId.Value))
            {
                student.GroupId = null;
                updateList.Add(student);
            }
        }

        var result = await schoolDal.ExecuteWithTransactionAsync(async () =>
        {
            var added = await schoolDal.UpdateAsyncCallback(school, cancellationToken: cancellationToken);
            await userDal.UpdateAsync(user, cancellationToken: cancellationToken);
            await schoolGroupDal.DeleteRangeAsync(deleteList, cancellationToken: cancellationToken);
            await schoolGroupDal.AddRangeAsync(schoolGroups, cancellationToken: cancellationToken);
            await userDal.UpdateRangeAsync(updateList, cancellationToken: cancellationToken);
            var result = mapper.Map<GetSchoolModel>(added);
            return result;
        }, cancellationToken: cancellationToken);

        return result;
    }
}

public class UpdateSchoolCommandValidator : AbstractValidator<UpdateSchoolCommand>
{
    public UpdateSchoolCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotNull().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.Name).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Model.Name).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Model.Name).MaximumLength(250).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "250"]);

        RuleFor(x => x.Model.TaxNumber).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.TaxNumber]);
        RuleFor(x => x.Model.TaxNumber).MinimumLength(10).WithMessage(Strings.DynamicMinLength, [Strings.TaxNumber, "10"]);
        RuleFor(x => x.Model.TaxNumber).MaximumLength(11).WithMessage(Strings.DynamicMaxLength, [Strings.TaxNumber, "11"]);

        RuleFor(x => x.Model.Address).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Address]);
        RuleFor(x => x.Model.Address).MinimumLength(3).WithMessage(Strings.DynamicMinLength, [Strings.Address, "3"]);

        RuleFor(x => x.Model.City).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.City]);
        RuleFor(x => x.Model.City).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.City, "2"]);
        RuleFor(x => x.Model.City).MaximumLength(50).WithMessage(Strings.DynamicMaxLength, [Strings.City, "50"]);

        RuleFor(x => x.Model.AuthorizedName).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [$"{Strings.Authorized} {Strings.OfName}"]);
        RuleFor(x => x.Model.AuthorizedName).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [$"{Strings.Authorized} {Strings.OfName}", "2"]);
        RuleFor(x => x.Model.AuthorizedName).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [$"{Strings.Authorized} {Strings.OfName}", "100"]);

        RuleFor(x => x.Model.AuthorizedEmail).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [$"{Strings.Authorized} {Strings.OfEmail}"]);
        RuleFor(x => x.Model.AuthorizedEmail).MinimumLength(5).WithMessage(Strings.DynamicMinLength, [$"{Strings.Authorized} {Strings.OfEmail}", "5"]);
        RuleFor(x => x.Model.AuthorizedEmail).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [$"{Strings.Authorized} {Strings.OfEmail}", "100"]);
        RuleFor(x => x.Model.AuthorizedEmail).EmailAddress().WithMessage(Strings.EmailWrongFormat);

        RuleFor(x => x.Model.AuthorizedPhone).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [$"{Strings.Authorized} {Strings.OfPhone}"]);
        RuleFor(x => x.Model.AuthorizedPhone).MinimumLength(10).WithMessage(Strings.DynamicMinLength, [$"{Strings.Authorized} {Strings.OfPhone}", "10"]);
        RuleFor(x => x.Model.AuthorizedPhone).MaximumLength(15).WithMessage(Strings.DynamicMaxLength, [$"{Strings.Authorized} {Strings.OfPhone}", "15"]);
        RuleFor(x => x.Model.AuthorizedPhone).Must(x => double.TryParse(x, out _)).WithMessage(Strings.DynamicOnlyDigit, [$"{Strings.Authorized} {Strings.OfPhone}"]);

        RuleFor(x => x.Model.UserCount).GreaterThan(0).WithMessage(Strings.DynamicGratherThan, [Strings.UserCount, "0"]);

        RuleFor(x => x.Model.LicenseEndDate).GreaterThan(DateTime.Now).WithMessage(Strings.DynamicGratherThan, [Strings.LicenseEndDate, $"{DateTime.Today:dd/MM/yyyy}"]);
    }
}