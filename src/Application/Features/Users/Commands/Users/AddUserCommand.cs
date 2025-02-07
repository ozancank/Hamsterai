using Application.Features.Packages.Rules;
using Application.Features.Users.Models.User;
using Application.Features.Users.Rules;
using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using Domain.Entities.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Caching;
using OCK.Core.Pipelines.Logging;
using OCK.Core.Security.HashingHelper;

namespace Application.Features.Users.Commands.Users;

public class AddUserCommand : IRequest<GetUserModel>, ISecuredRequest<UserTypes>, ILoggableRequest, ICacheRemoverRequest
{
    public required AddUserModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.School, UserTypes.Teacher];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } =
        [
            $"{nameof(Model)}.{nameof(Model.Password)}",
            $"{nameof(Model)}.{nameof(Model.ProfilePictureBase64)}"
        ];
    public string[] CacheKey { get; } = [$"^{Strings.CacheStatusAndLicence}"];
}

public class AddUserCommandHandler(IMapper mapper,
                                   ICommonService commonService,
                                   IUserDal userDal,
                                   IPackageUserDal packageUserDal,
                                   UserRules userRules,
                                   PackageRules packageRules) : IRequestHandler<AddUserCommand, GetUserModel>
{
    public async Task<GetUserModel> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        request.Model!.Email = request.Model.Email!.Trim().ToLower();
        request.Model.Password = request.Model.Password!.Trim();

        await userRules.UserNameCanNotBeDuplicated(request.Model.Email);
        await userRules.UserEmailCanNotBeDuplicated(request.Model.Email);
        await userRules.UserTypeAllowed(request.Model.Type);

        var id = await userDal.GetNextPrimaryKeyAsync(x => x.Id, cancellationToken: cancellationToken);
        var date = DateTime.Now;
        if (request.Model.ProfilePictureFileName.IsNotEmpty() && request.Model.ProfilePictureBase64.IsNotEmpty())
        {
            var extension = Path.GetExtension(request.Model.ProfilePictureFileName);
            var fileName = $"P_{id}_{Guid.NewGuid()}{extension}";
            await commonService.PictureConvert(request.Model.ProfilePictureBase64, fileName, AppOptions.ProfilePictureFolderPath, cancellationToken);
            request.Model.ProfileUrl = fileName;
        }

        HashingHelper.CreatePasswordHash(request.Model.Password, out byte[] passwordHash, out byte[] passwordSalt);

        if (request.Model.ExitPassword.IsNotEmpty())
            request.Model.ExitPassword = CryptographyTools.EncryptWithAes256(request.Model.ExitPassword, AppOptions.ExitPassKeyword, AppOptions.ExitPassVector);

        var user = new User
        {
            Id = id,
            UserName = request.Model.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            MustPasswordChange = false,
            CreateDate = date,
            IsActive = true,
            Name = request.Model.Name,
            Surname = request.Model.Surname,
            Phone = request.Model.Phone?.TrimForPhone(),
            ProfileUrl = request.Model.ProfileUrl,
            Email = request.Model.Email,
            Type = Enum.Parse<UserTypes>($"{request.Model.Type}"),
            ConnectionId = request.Model.ConnectionId,
            SchoolId = request.Model.SchoolId,
            TaxNumber = request.Model.TaxNumber,
            ExitPassword = request.Model.ExitPassword.IfNullEmptyString(Strings.DefaultExitPassCipher),
        };

        var packageUsers = new List<PackageUser>();

        foreach (var packageId in request.Model.PackageIds)
        {
            await packageRules.PackageShouldExistsById(packageId);

            var packageUser = new PackageUser
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                CreateUser = commonService.HttpUserId,
                CreateDate = date,
                UpdateUser = commonService.HttpUserId,
                UpdateDate = date,
                UserId = id,
                PackageId = packageId,
                EndDate = request.Model.LicenceEndDate,
                QuestionCredit = request.Model.QuestionCredit,
            };
            packageUsers.Add(packageUser);
        }

        var result = await userDal.ExecuteWithTransactionAsync(async () =>
        {
            await userDal.AddAsync(user, cancellationToken: cancellationToken);
            await packageUserDal.AddRangeAsync(packageUsers, cancellationToken: cancellationToken);
            var result = mapper.Map<GetUserModel>(user);
            return result;
        }, cancellationToken: cancellationToken);
        return result;
    }
}

public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
{
    public AddUserCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.Password)
                .NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Password])
                .MinimumLength(8).WithMessage(Strings.DynamicMinLength, [Strings.Password, "8"])
                .Matches("[a-zA-Z]").WithMessage(Strings.PasswordLetter)
                .Matches("[0-9]").WithMessage(Strings.PasswordNumber);

        RuleFor(x => x.Model.Name).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Model.Name).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Model.Name).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "100"]);

        RuleFor(x => x.Model.Surname).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Surname]);
        RuleFor(x => x.Model.Surname).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Surname, "2"]);
        RuleFor(x => x.Model.Surname).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [Strings.Surname, "100"]);

        //RuleFor(x => x.Phone).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Phone]);
        //RuleFor(x => x.Phone.EmptyOrTrim(" ")).MinimumLength(10).WithMessage(Strings.DynamicMinLength, [Strings.Phone, "10"]);
        RuleFor(x => x.Model.Phone.EmptyOrTrim(" ")).MaximumLength(15).WithMessage(Strings.DynamicMaxLength, [Strings.Phone, "15"]);
        //RuleFor(x => x.Phone).Must(x => double.TryParse(x, out _)).WithMessage(Strings.DynamicOnlyDigit, [Strings.PhoneNumber]);

        RuleFor(x => x.Model.Email).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Email]);
        RuleFor(x => x.Model.Email).EmailAddress().WithMessage(Strings.EmailWrongFormat);
        RuleFor(x => x.Model.Email).MinimumLength(6).WithMessage(Strings.DynamicMinLength, [Strings.Email, "6"]);
        RuleFor(x => x.Model.Email).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [Strings.Email, "100"]);

        RuleFor(x => (byte)x.Model.Type).InclusiveBetween((byte)1, (byte)5).WithMessage(Strings.DynamicBetween, [Strings.UserType, "1", "5"]);

        RuleFor(x => x.Model.TaxNumber).MaximumLength(11).WithMessage(Strings.DynamicMaxLength, [Strings.TaxNumber, "11"]);
    }
}