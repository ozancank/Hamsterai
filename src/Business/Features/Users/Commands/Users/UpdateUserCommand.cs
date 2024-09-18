using Business.Features.Users.Models.User;
using Business.Features.Users.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Users.Commands.Users;

public class UpdateUserCommand : IRequest<GetUserModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public UpdateUserModel UpdateUserModel { get; set; }
    public UserTypes[] Roles { get; } = [];
    public string[] HidePropertyNames { get; } = ["UpdateUserModel.Password", "UpdateUserModel.ProfilePictureBase64"];
}

public class UpdateUserCommandHandler(IMapper mapper,
                                      ICommonService commonService,
                                      IUserDal userDal,
                                      UserRules userRules) : IRequestHandler<UpdateUserCommand, GetUserModel>
{
    public async Task<GetUserModel> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userDal.GetAsync(predicate: x => x.Id == request.UpdateUserModel.Id, cancellationToken: cancellationToken);

        await UserRules.UserShouldExistsAndActive(user);
        await userRules.UserNameCanNotBeDuplicated(request.UpdateUserModel.UserName);
        await userRules.UserEmailCanNotBeDuplicated(request.UpdateUserModel.Email);
        await userRules.UserPhoneCanNotBeDuplicated(request.UpdateUserModel.Phone);
        await userRules.UserTypeAllowed(user.Type, user.Id);

        var path = await commonService.PictureConvert(request.UpdateUserModel.ProfilePictureBase64, request.UpdateUserModel.ProfilePictureFileName, AppOptions.ProfilePictureFolderPath);
        if (path.Item1.IsNotEmpty()) request.UpdateUserModel.ProfileUrl = path.Item1;

        user.UserName = request.UpdateUserModel.UserName?.Trim();
        user.Name = request.UpdateUserModel.Name;
        user.Surname = request.UpdateUserModel.Surname;
        user.Phone = request.UpdateUserModel.Phone;
        user.ProfileUrl = request.UpdateUserModel.ProfileUrl;
        user.Email = request.UpdateUserModel.Email;
        user.Type = Enum.Parse<UserTypes>($"{request.UpdateUserModel.Type}");
        user.ConnectionId = request.UpdateUserModel.ConnectionId;
        user.SchoolId = request.UpdateUserModel.SchoolId;

        await userDal.UpdateAsync(user);
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
    }
}