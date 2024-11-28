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
                                             IPackageUserDal packageUserDal,
                                             UserRules userRules,
                                             PackageRules packageRules) : IRequestHandler<UpdateUserCommand, GetUserModel>
{
    public async Task<GetUserModel> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        request.Model.Email = request.Model.Email!.Trim().ToLower();

        var user = await userDal.GetAsync(predicate: x => x.Id == request.Model.Id, cancellationToken: cancellationToken);

        await UserRules.UserShouldExistsAndActive(user);
        await userRules.UserNameCanNotBeDuplicated(request.Model.Email, request.Model.Id);
        await userRules.UserEmailCanNotBeDuplicated(request.Model.Email, request.Model.Id);
        await userRules.UserTypeAllowed(user.Type, user.Id);

        var date = DateTime.Now;
        if (request.Model.ProfilePictureFileName.IsNotEmpty() && request.Model.ProfilePictureBase64.IsNotEmpty())
        {
            var extension = Path.GetExtension(request.Model.ProfilePictureFileName);
            var fileName = $"P_{request.Model.Id}_{Guid.NewGuid()}{extension}";
            await commonService.PictureConvert(request.Model.ProfilePictureBase64, request.Model.ProfilePictureFileName, AppOptions.ProfilePictureFolderPath, cancellationToken);
            request.Model.ProfileUrl = fileName;
        }

        user.UserName = request.Model.Email;
        user.Name = request.Model.Name;
        user.Surname = request.Model.Surname;
        user.Phone = request.Model.Phone?.TrimForPhone();
        user.ProfileUrl = request.Model.ProfileUrl;
        user.Email = request.Model.Email;
        user.TaxNumber = request.Model.TaxNumber;

        var existingPackageUsers = new List<Guid>();
        var packageUsers = new List<PackageUser>();
        if (commonService.HttpUserType == UserTypes.Administator)
        {
            user.Type = Enum.Parse<UserTypes>($"{request.Model.Type}");
            user.SchoolId = request.Model.SchoolId;
            user.ConnectionId = request.Model.ConnectionId;

            existingPackageUsers = await packageUserDal.GetListAsync(predicate: x => x.UserId == user.Id, selector: x => x.Id, cancellationToken: cancellationToken);

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
                    UserId = user.Id,
                    PackageId = packageId,
                    EndDate = request.Model.LicenceEndDate,
                };
                packageUsers.Add(packageUser);
            }

            if (packageUsers.Count != 0)
            {
                packageUsers[0].QuestionCredit = request.Model.QuestionCredit;
            }
        }

        var result = await userDal.ExecuteWithTransactionAsync(async () =>
        {
            await userDal.UpdateAsync(user, cancellationToken: cancellationToken);
            if (commonService.HttpUserType == UserTypes.Administator)
            {
                await packageUserDal.DeleteRangeAsync(existingPackageUsers, cancellationToken: cancellationToken);
                await packageUserDal.AddRangeAsync(packageUsers, cancellationToken: cancellationToken);
            }
            var result = mapper.Map<GetUserModel>(user);
            return result;
        }, cancellationToken: cancellationToken);
        return result;
    }
}

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.Id).NotEmpty().WithMessage(Strings.IdNotEmpty);

        RuleFor(x => x.Model.Name).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Model.Name).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Model.Name).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "100"]);

        RuleFor(x => x.Model.Surname).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Surname]);
        RuleFor(x => x.Model.Surname).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Surname, "2"]);
        RuleFor(x => x.Model.Surname).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [Strings.Surname, "100"]);

        //RuleFor(x => x.Phone).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Phone]);
        //RuleFor(x => x.Phone.EmptyOrTrim(" ")).MinimumLength(10).WithMessage(Strings.DynamicMinLength, [Strings.Phone, "10"]);
        RuleFor(x => x.Model.Phone.EmptyOrTrim(" ")).MaximumLength(10).WithMessage(Strings.DynamicMaxLength, [Strings.Phone, "15"]);
        //RuleFor(x => x.Phone).Must(x => double.TryParse(x, out _)).WithMessage(Strings.DynamicOnlyDigit, ["Telefon Numarası"]);

        RuleFor(x => x.Model.Email).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Email]);
        RuleFor(x => x.Model.Email).EmailAddress().WithMessage(Strings.EmailWrongFormat);
        RuleFor(x => x.Model.Email).MinimumLength(6).WithMessage(Strings.DynamicMinLength, [Strings.Email, "6"]);
        RuleFor(x => x.Model.Email).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [Strings.Email, "100"]);

        RuleFor(x => (byte)x.Model.Type).InclusiveBetween((byte)1, (byte)5).WithMessage(Strings.DynamicBetween, [Strings.UserType, "1", "5"]);

        RuleFor(x => x.Model.TaxNumber).MaximumLength(11).WithMessage(Strings.DynamicMaxLength, [Strings.TaxNumber, "11"]);
    }
}