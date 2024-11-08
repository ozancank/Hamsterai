using Business.Features.Packages.Rules;
using Business.Features.Users.Models.User;
using Business.Features.Users.Rules;
using Business.Services.CommonService;
using DataAccess.Abstract.Core;
using Domain.Entities.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Caching;
using OCK.Core.Pipelines.Logging;
using OCK.Core.Security.HashingHelper;

namespace Business.Features.Users.Commands.Users;

public class AddUserCommand : IRequest<GetUserModel>, ISecuredRequest<UserTypes>, ILoggableRequest, ICacheRemoverRequest
{
    public required AddUserModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.School, UserTypes.Teacher];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = ["AddUserModel.Password", "AddUserModel.ProfilePictureBase64"];
    public string[] CacheKey { get; } = [$"^{Strings.CacheStatusAndLicence}"];
}

public class AddUserCommandHandler(IMapper mapper,
                                   ICommonService commonService,
                                   IUserDal userDal,
                                   UserRules userRules,
                                   PackageRules packageRules) : IRequestHandler<AddUserCommand, GetUserModel>
{
    public async Task<GetUserModel> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        request.Model!.UserName = request.Model.UserName!.Trim().ToLower();
        request.Model.Password = request.Model.Password!.Trim();

        await userRules.UserNameCanNotBeDuplicated(request.Model.UserName);
        await userRules.UserEmailCanNotBeDuplicated(request.Model.Email!);
        await userRules.UserPhoneCanNotBeDuplicated(request.Model.Phone!);
        await userRules.UserTypeAllowed(request.Model.Type);
        await packageRules.PackageShouldExistsById(request.Model.PackageId);

        var id = await userDal.GetNextPrimaryKeyAsync(x => x.Id, cancellationToken: cancellationToken);
        var date = DateTime.Now;
        if (request.Model.ProfilePictureFileName.IsNotEmpty() && request.Model.ProfilePictureBase64.IsNotEmpty())
        {
            var extension = Path.GetExtension(request.Model.ProfilePictureFileName);
            var fileName = $"P_{id}_{Guid.NewGuid()}{extension}";
            await commonService.PictureConvert(request.Model.ProfilePictureBase64, request.Model.ProfilePictureFileName, AppOptions.ProfilePictureFolderPath);
            request.Model.ProfileUrl = fileName;
        }

        HashingHelper.CreatePasswordHash(request.Model.Password, out byte[] passwordHash, out byte[] passwordSalt);
        var user = new User
        {
            Id = id,
            UserName = request.Model.UserName?.Trim(),
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            MustPasswordChange = false,
            CreateDate = date,
            IsActive = true,
            Name = request.Model.Name,
            Surname = request.Model.Surname,
            Phone = request.Model.Phone?.Trim(),
            ProfileUrl = request.Model.ProfileUrl,
            Email = request.Model.Email?.Trim(),
            Type = Enum.Parse<UserTypes>($"{request.Model.Type}"),
            ConnectionId = request.Model.ConnectionId,
            SchoolId = request.Model.SchoolId,
            //GroupId = request.Model.PackageId,
            PackageCredit = request.Model.PackageCredit,
            AddtionalCredit = request.Model.AddtionalCredit,
            TaxNumber = request.Model.TaxNumber,
            LicenceEndDate = request.Model.LicenceEndDate
        };

        await userDal.AddAsync(user, cancellationToken: cancellationToken);
        var result = mapper.Map<GetUserModel>(user);
        return result;
    }
}

public class AddUserCommandValidator : AbstractValidator<AddUserModel>
{
    public AddUserCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.UserName).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.UserName]);
        RuleFor(x => x.UserName.EmptyOrTrim()).Must(x => !x.Trim().Contains(' ')).WithMessage(Strings.UserNameSpace);

        RuleFor(x => x.Password)
                .NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Password])
                .MinimumLength(8).WithMessage(Strings.DynamicMinLength, [Strings.Password, "8"])
                .Matches("[a-zA-Z]").WithMessage(Strings.PasswordLetter)
                .Matches("[0-9]").WithMessage(Strings.PasswordNumber);

        RuleFor(x => x.Name).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Name).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Name).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "100"]);

        RuleFor(x => x.Surname).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Surname]);
        RuleFor(x => x.Surname).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Surname, "2"]);
        RuleFor(x => x.Surname).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [Strings.Surname, "100"]);

        RuleFor(x => x.Phone).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Phone]);
        RuleFor(x => x.Phone.EmptyOrTrim(" ")).MinimumLength(10).WithMessage(Strings.DynamicMinLength, [Strings.Phone, "10"]);
        RuleFor(x => x.Phone.EmptyOrTrim(" ")).MaximumLength(10).WithMessage(Strings.DynamicMaxLength, [Strings.Phone, "15"]);
        RuleFor(x => x.Phone).Must(x => double.TryParse(x, out _)).WithMessage(Strings.DynamicOnlyDigit, [Strings.PhoneNumber]);

        RuleFor(x => x.Email).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Email]);
        RuleFor(x => x.Email).EmailAddress().WithMessage(Strings.EmailWrongFormat);
        RuleFor(x => x.Email).MinimumLength(6).WithMessage(Strings.DynamicMinLength, [Strings.Email, "6"]);
        RuleFor(x => x.Email).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [Strings.Email, "100"]);

        RuleFor(x => (byte)x.Type).InclusiveBetween((byte)1, (byte)4).WithMessage(Strings.DynamicBetween, [Strings.UserType, "1", "4"]);

        RuleFor(x => x.PackageId).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Package]);
        RuleFor(x => x.PackageId).InclusiveBetween((byte)1, (byte)255).WithMessage(Strings.DynamicBetween, [Strings.Package, "1", "255"]);
    }
}