using Business.Features.Users.Rules;
using Business.Services.EmailService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Users.Commands.Password;

public class UserSendForgetPasswordEmailCommand : IRequest<bool>, ILoggableRequest
{
    public string Email { get; set; }

    public string[] HidePropertyNames { get; } = [];
}

public class UserSendForgetPasswordEmailCommandHandler(IUserDal userDal,
                                                       IEmailService emailService,
                                                       IPasswordTokenDal passwordTokenDal) : IRequestHandler<UserSendForgetPasswordEmailCommand, bool>
{
    public async Task<bool> Handle(UserSendForgetPasswordEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userDal.GetAsync(
            predicate: x => x.Email == request.Email,
            include: x => x.Include(u => u.PasswordTokens),
            cancellationToken: cancellationToken);

        await UserRules.UserShouldExistsAndActive(user);

        if (user.PasswordTokens.Count != 0) await passwordTokenDal.DeleteRangeAsync(user.PasswordTokens, cancellationToken);

        var passwordToken = new PasswordToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = $"{Guid.NewGuid()}-{Guid.NewGuid()}".Trim("-"),
            CreateDate = DateTime.Now,
            Expried = DateTime.Now.AddHours(1)
        };

        await passwordTokenDal.AddAsync(passwordToken, cancellationToken: cancellationToken);

        var link = $"{AppOptions.ForgetPasswordUrl}/{passwordToken.Token}";

        await emailService.SendForgetPassword(request.Email, link);

        return true;
    }
}

public class UserSendForgetPasswordEmailCommandValidator : AbstractValidator<UserSendForgetPasswordEmailCommand>
{
    public UserSendForgetPasswordEmailCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Email]);
        RuleFor(x => x.Email).EmailAddress().WithMessage(Strings.EmailWrongFormat);
        RuleFor(x => x.Email).MinimumLength(6).WithMessage(Strings.DynamicMinLength, [Strings.Email, "6"]);
        RuleFor(x => x.Email).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [Strings.Email, "100"]);
    }
}