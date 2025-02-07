using Application.Features.Packages.Rules;
using Application.Features.Schools.Models.Schools;
using Application.Features.Schools.Rules;
using Application.Features.Users.Rules;
using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using Domain.Entities.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Caching;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Schools.Commands.Schools;

public class UpdateSchoolCommand : IRequest<GetSchoolModel>, ISecuredRequest<UserTypes>, ILoggableRequest, ICacheRemoverRequest
{
    public required UpdateSchoolModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
    public string[] CacheKey { get; } = [$"^{Strings.CacheStatusAndLicence}"];
}

public class UpdateSchoolCommandHandler(IMapper mapper,
                                        ISchoolDal schoolDal,
                                        ICommonService commonService,
                                        IUserDal userDal,
                                        IPackageDal packageDal,
                                        IPackageUserDal packageUserDal,
                                        UserRules userRules,
                                        SchoolRules schoolRules,
                                        PackageRules packageRules) : IRequestHandler<UpdateSchoolCommand, GetSchoolModel>
{
    public async Task<GetSchoolModel> Handle(UpdateSchoolCommand request, CancellationToken cancellationToken)
    {
        var school = await schoolDal.GetAsync(x => x.Id == request.Model.Id, cancellationToken: cancellationToken);

        await SchoolRules.SchoolShouldExists(school);
        await schoolRules.SchoolNameAndCityCanNotBeDuplicated(request.Model.Name!, request.Model.City!, school.Id);
        await schoolRules.SchoolTaxNumberCanNotBeDuplicated(request.Model.TaxNumber!, school.Id);
        await packageRules.PackageShouldBeRecordInDatabase(request.Model.PackageIds);

        var user = await userDal.GetAsync(x => x.SchoolId == school.Id && x.Type == UserTypes.School, cancellationToken: cancellationToken);

        await UserRules.UserShouldExists(user);
        await userRules.UserNameCanNotBeDuplicated(request.Model.AuthorizedEmail!, user.Id);
        await userRules.UserEmailCanNotBeDuplicated(request.Model.AuthorizedEmail!, user.Id);
        await userRules.UserPhoneCanNotBeDuplicated(request.Model.AuthorizedPhone!, user.Id);

        var userId = commonService.HttpUserId;
        var date = DateTime.Now;

        school.Name = request.Model.Name;
        school.TaxNumber = request.Model.TaxNumber;
        school.Address = request.Model.Address;
        school.City = request.Model.City;
        school.AuthorizedName = request.Model.AuthorizedName;
        school.AuthorizedPhone = request.Model.AuthorizedPhone;
        school.AuthorizedEmail = request.Model.AuthorizedEmail;
        school.UserCount = request.Model.UserCount;

        user.Name = school.Name;
        user.UserName = school.TaxNumber!.Trim().ToLower();
        user.Email = school.AuthorizedEmail!.Trim().ToLower();
        user.Phone = school.AuthorizedPhone.TrimForPhone();
        user.TaxNumber = school.TaxNumber;

        var usersInSchool = await userDal.GetListAsync(
            predicate: x => x.SchoolId == school.Id,
            include: x => x.Include(u => u.PackageUsers).ThenInclude(u => u.Package),
            selector: x => x.Id,
            cancellationToken: cancellationToken);

        var deleteList = await packageUserDal.GetListAsync(predicate: x => x.UserId == user.Id || usersInSchool.Contains(x.UserId), cancellationToken: cancellationToken);

        var packages = await packageDal.GetListAsync(predicate: x => request.Model.PackageIds.Contains(x.Id), cancellationToken: cancellationToken);

        var packageUsers = request.Model.PackageIds.Select(x => new PackageUser
        {
            Id = Guid.NewGuid(),
            IsActive = true,
            CreateUser = userId,
            CreateDate = date,
            UpdateUser = userId,
            UpdateDate = date,
            UserId = user.Id,
            PackageId = x,
            EndDate = request.Model.LicenseEndDate,
            QuestionCredit = packages.FirstOrDefault(p => p.Id == x)?.QuestionCredit ?? 0,
        }).ToList();

        var updateList = new List<User>();

        //foreach (var studentUser in usersInSchool)
        //{
        //    if (studentUser.PackageUsers.Count > 0 && !studentUser.PackageUsers.Any(x => request.Model.PackageIds.Contains(x.PackageId)))
        //    {
        //        //studentUser.PackageId = null;
        //        updateList.Add(studentUser);
        //    }
        //}

        var result = await schoolDal.ExecuteWithTransactionAsync(async () =>
        {
            var added = await schoolDal.UpdateAsyncCallback(school, cancellationToken: cancellationToken);
            await userDal.UpdateAsync(user, cancellationToken: cancellationToken);
            await packageUserDal.DeleteRangeAsync(deleteList, cancellationToken: cancellationToken);
            await packageUserDal.AddRangeAsync(packageUsers, cancellationToken: cancellationToken);
            await userDal.UpdateRangeAsync(updateList, cancellationToken: cancellationToken);
            var result = await schoolDal.GetAsyncAutoMapper<GetSchoolModel>(
                enableTracking: false,
                predicate: x => x.Id == school.Id,
                include: x => x.Include(u => u.Users).ThenInclude(u => u.PackageUsers).ThenInclude(u => u.Package),
                configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);
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

        RuleFor(x => x.Model.UserCount).GreaterThan(0).WithMessage(Strings.DynamicGreaterThan, [Strings.UserCount, "0"]);

        RuleFor(x => x.Model.LicenseEndDate).GreaterThan(DateTime.Now).WithMessage(Strings.DynamicGreaterThan, [Strings.LicenseEndDate, $"{DateTime.Today:dd/MM/yyyy}"]);
    }
}