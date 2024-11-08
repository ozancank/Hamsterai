using Application.Features.Packages.Rules;
using Application.Features.Users.Models.User;
using Application.Features.Users.Rules;
using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Caching;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Users.Commands.Users;

public sealed class UpdateUserCommand : IRequest<GetUserModel>, ISecuredRequest<UserTypes>, ILoggableRequest, ICacheRemoverRequest
{
    public required UpdateUserModel Model { get; set; }
    public UserTypes[] Roles { get; } = [];
    public string[] HidePropertyNames { get; } = ["UpdateUserModel.Password", "UpdateUserModel.ProfilePictureBase64"];
    public bool AllowByPass => false;
    public string[] CacheKey { get; } = [$"^{Strings.CacheStatusAndLicence}"];
}

public sealed class UpdateUserCommandHandler(IMapper mapper,
                                             ICommonService commonService,
                                             IUserDal userDal,
                                             UserRules userRules,
                                             PackageRules packageRules) : IRequestHandler<UpdateUserCommand, GetUserModel>
{
    public async Task<GetUserModel> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userDal.GetAsync(predicate: x => x.Id == request.Model.Id, cancellationToken: cancellationToken);

        await UserRules.UserShouldExistsAndActive(user);
        await userRules.UserNameCanNotBeDuplicated(request.Model.UserName, request.Model.Id);
        await userRules.UserEmailCanNotBeDuplicated(request.Model.Email, request.Model.Id);
        await userRules.UserPhoneCanNotBeDuplicated(request.Model.Phone, request.Model.Id);
        await userRules.UserTypeAllowed(user.Type, user.Id);
        await packageRules.PackageShouldExistsById(request.Model.PackageId);

        var date = DateTime.Now;
        if (request.Model.ProfilePictureFileName.IsNotEmpty() && request.Model.ProfilePictureBase64.IsNotEmpty())
        {
            var extension = Path.GetExtension(request.Model.ProfilePictureFileName);
            var fileName = $"P_{request.Model.Id}_{Guid.NewGuid()}{extension}";
            await commonService.PictureConvert(request.Model.ProfilePictureBase64, request.Model.ProfilePictureFileName, AppOptions.ProfilePictureFolderPath);
            request.Model.ProfileUrl = fileName;
        }

        user.UserName = request.Model.UserName?.Trim();
        user.Name = request.Model.Name;
        user.Surname = request.Model.Surname;
        user.Phone = request.Model.Phone;
        user.ProfileUrl = request.Model.ProfileUrl;
        user.Email = request.Model.Email;
        user.Type = Enum.Parse<UserTypes>($"{request.Model.Type}");
        user.ConnectionId = request.Model.ConnectionId;
        user.SchoolId = request.Model.SchoolId;
        //user.GroupId = request.Model.PackageId;
        user.PackageCredit = request.Model.PackageCredit;
        user.AddtionalCredit = request.Model.AddtionalCredit;
        user.TaxNumber = request.Model.TaxNumber;
        user.LicenceEndDate = request.Model.LicenceEndDate;

        await userDal.UpdateAsync(user, cancellationToken: cancellationToken);
        var result = mapper.Map<GetUserModel>(user);
        return result;
    }
}

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserModel>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Id).NotEmpty().WithMessage(Strings.IdNotEmpty);

        RuleFor(x => x.UserName.Trim()).Must(x => !x.Trim().Contains(' ')).WithMessage(Strings.UserNameSpace);

        RuleFor(x => x.Name).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Name).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Name).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "100"]);

        RuleFor(x => x.Surname).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Surname]);
        RuleFor(x => x.Surname).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Surname, "2"]);
        RuleFor(x => x.Surname).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [Strings.Surname, "100"]);

        RuleFor(x => x.Phone).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Phone]);
        RuleFor(x => x.Phone.EmptyOrTrim(" ")).MinimumLength(10).WithMessage(Strings.DynamicMinLength, [Strings.Phone, "10"]);
        RuleFor(x => x.Phone.EmptyOrTrim(" ")).MaximumLength(10).WithMessage(Strings.DynamicMaxLength, [Strings.Phone, "15"]);
        RuleFor(x => x.Phone).Must(x => double.TryParse(x, out _)).WithMessage(Strings.DynamicOnlyDigit, ["Telefon Numarası"]);

        RuleFor(x => x.Email).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Email]);
        RuleFor(x => x.Email).EmailAddress().WithMessage(Strings.EmailWrongFormat);
        RuleFor(x => x.Email).MinimumLength(6).WithMessage(Strings.DynamicMinLength, [Strings.Email, "6"]);
        RuleFor(x => x.Email).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [Strings.Email, "100"]);

        RuleFor(x => (byte)x.Type).InclusiveBetween((byte)1, (byte)4).WithMessage(Strings.DynamicBetween, [Strings.UserType, "1", "4"]);

        RuleFor(x => x.PackageId).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Package]);
        RuleFor(x => x.PackageId).InclusiveBetween((byte)1, (byte)255).WithMessage(Strings.DynamicBetween, [Strings.Package, "1", "255"]);
    }
}