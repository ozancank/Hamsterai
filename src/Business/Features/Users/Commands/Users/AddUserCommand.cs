using Business.Features.Users.Models.User;
using Business.Features.Users.Rules;
using Business.Services.CommonService;
using Domain.Entities.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;
using OCK.Core.Security.HashingHelper;

namespace Business.Features.Users.Commands.Users;

public class AddUserCommand : IRequest<GetUserModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public AddUserModel AddLessonModel { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.School, UserTypes.Teacher];
    public string[] HidePropertyNames { get; } = ["AddUserModel.Password", "AddUserModel.ProfilePictureBase64"];
}

public class AddUserCommandHandler(IMapper mapper,
                                   ICommonService commonService,
                                   IUserDal userDal,
                                   UserRules userRules) : IRequestHandler<AddUserCommand, GetUserModel>
{
    public async Task<GetUserModel> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        request.AddLessonModel!.UserName = request.AddLessonModel.UserName!.Trim().ToLower();
        request.AddLessonModel.Password = request.AddLessonModel.Password!.Trim();

        await userRules.UserNameCanNotBeDuplicated(request.AddLessonModel.UserName);
        await userRules.UserEmailCanNotBeDuplicated(request.AddLessonModel.Email);
        await userRules.UserPhoneCanNotBeDuplicated(request.AddLessonModel.Phone);
        await userRules.UserTypeAllowed(request.AddLessonModel.Type);

        var path = await commonService.PictureConvert(request.AddLessonModel.ProfilePictureBase64, request.AddLessonModel.ProfilePictureFileName, AppOptions.ProfilePictureFolderPath);
        if (path.Item1.IsNotEmpty()) request.AddLessonModel.ProfileUrl = path.Item1;

        HashingHelper.CreatePasswordHash(request.AddLessonModel.Password, out byte[] passwordHash, out byte[] passwordSalt);
        var user = new User
        {
            Id = await userDal.GetNextPrimaryKeyAsync(x => x.Id, cancellationToken: cancellationToken),
            UserName = request.AddLessonModel.UserName?.Trim(),
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            MustPasswordChange = false,
            CreateDate = DateTime.Now,
            IsActive = true,
            Name = request.AddLessonModel.Name,
            Surname = request.AddLessonModel.Surname,
            Phone = request.AddLessonModel.Phone?.Trim(),
            ProfileUrl = request.AddLessonModel.ProfileUrl?.Trim(),
            Email = request.AddLessonModel.Email?.Trim(),
            Type = Enum.Parse<UserTypes>($"{request.AddLessonModel.Type}"),
            ConnectionId = request.AddLessonModel.ConnectionId,
            SchoolId = request.AddLessonModel.SchoolId,
        };

        await userDal.AddAsync(user);
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
        RuleFor(x => x.UserName.Trim()).Must(x => !x.Trim().Contains(' ')).WithMessage(Strings.UserNameSpace);

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
    }
}