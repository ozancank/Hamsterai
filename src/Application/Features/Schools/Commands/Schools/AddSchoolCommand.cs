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
using OCK.Core.Security.HashingHelper;

namespace Application.Features.Schools.Commands.Schools;

public class AddSchoolCommand : IRequest<GetSchoolModel>, ISecuredRequest<UserTypes>, ILoggableRequest, ICacheRemoverRequest
{
    public required AddSchoolModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
    public string[] CacheKey { get; } = [$"^{Strings.CacheStatusAndLicence}"];
}

public class AddSchoolCommandHandler(IMapper mapper,
                                     ISchoolDal schoolDal,
                                     ICommonService commonService,
                                     IUserDal userDal,
                                     IPackageUserDal packageUserDal,
                                     UserRules userRules,
                                     SchoolRules schoolRules,
                                     PackageRules packageRules) : IRequestHandler<AddSchoolCommand, GetSchoolModel>
{
    public async Task<GetSchoolModel> Handle(AddSchoolCommand request, CancellationToken cancellationToken)
    {
        await schoolRules.SchoolNameAndCityCanNotBeDuplicated(request.Model.Name!, request.Model.City!);
        await schoolRules.SchoolTaxNumberCanNotBeDuplicated(request.Model.TaxNumber!);
        await userRules.UserNameCanNotBeDuplicated(request.Model.TaxNumber!);

        await userRules.UserEmailCanNotBeDuplicated(request.Model.AuthorizedEmail!);
        await userRules.UserPhoneCanNotBeDuplicated(request.Model.AuthorizedPhone!);
        await packageRules.PackageShouldBeRecordInDatabase(request.Model.PackageIds);

        var userId = commonService.HttpUserId;
        var date = DateTime.Now;

        var school = mapper.Map<School>(request.Model);
        school.Id = await schoolDal.GetNextPrimaryKeyAsync(x => x.Id, cancellationToken: cancellationToken);
        school.IsActive = true;
        school.CreateUser = userId;
        school.CreateDate = date;
        school.UpdateUser = userId;
        school.UpdateDate = date;

        HashingHelper.CreatePasswordHash(AppOptions.DefaultPassword, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new User
        {
            Id = await userDal.GetNextPrimaryKeyAsync(x => x.Id, cancellationToken: cancellationToken),
            CreateDate = date,
            UserName = school.TaxNumber!.Trim().ToLower(),
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            MustPasswordChange = true,
            IsActive = true,
            Name = school.Name,
            Surname = string.Empty,
            Phone = school.AuthorizedPhone?.TrimForPhone(),
            ProfileUrl = string.Empty,
            Email = school.AuthorizedEmail!.Trim(),
            Type = UserTypes.School,
            SchoolId = school.Id,
            ConnectionId = null,
            TaxNumber = school.TaxNumber!.Trim(),
        };

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
            QuestionCredit = request.Model.QuestionCredit,
        }).ToList();

        var result = await schoolDal.ExecuteWithTransactionAsync(async () =>
        {
            var added = await schoolDal.AddAsyncCallback(school, cancellationToken: cancellationToken);
            await userDal.AddAsync(user, cancellationToken: cancellationToken);
            await packageUserDal.AddRangeAsync(packageUsers, cancellationToken: cancellationToken);
            var result = mapper.Map<GetSchoolModel>(added);
            return result;
        }, cancellationToken: cancellationToken);

        return result;
    }
}

public class AddSchoolCommandValidator : AbstractValidator<AddSchoolCommand>
{
    public AddSchoolCommandValidator()
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