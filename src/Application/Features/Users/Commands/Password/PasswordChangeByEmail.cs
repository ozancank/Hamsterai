using Application.Features.Users.Rules;
using DataAccess.Abstract.Core;
using Domain.Entities.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;
using OCK.Core.Security.HashingHelper;

namespace Application.Features.Users.Commands.Password;

public class PasswordChangeByEmailCommand : IRequest<bool>, ILoggableRequest
{
    public required string Password { get; set; }
    public required string Token { get; set; }

    public string[] HidePropertyNames { get; } = ["Password"];
}

public class PasswordChangeByEmailCommandHandler(IUserDal userDal,
                                                 IPasswordTokenDal passwordTokenDal) : IRequestHandler<PasswordChangeByEmailCommand, bool>
{
    public async Task<bool> Handle(PasswordChangeByEmailCommand request, CancellationToken cancellationToken)
    {
        User? user = null;
        try
        {
            var passwordToken = passwordTokenDal.Get(
                predicate: x => x.Token == request.Token,
                enableTracking: false);
            await UserRules.PasswordTokenShouldExists(passwordToken);

            user = await userDal.GetAsync(
                predicate: x => x.Id == passwordToken.UserId,
                include: x => x.Include(u => u.PasswordTokens),
                cancellationToken: cancellationToken);
            await UserRules.UserShouldExistsAndActive(user);
            await UserRules.IsPasswordTokenExpried(passwordToken.Expried);

            HashingHelper.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.MustPasswordChange = false;
            await userDal.UpdateAsync(user, cancellationToken: cancellationToken);
            return true;
        }
        finally
        {
            if (user != null)
                await passwordTokenDal.DeleteRangeAsync(user.PasswordTokens, cancellationToken);
        }
    }
}

public class PasswordChangeByEmailCommandHandlerValidator : AbstractValidator<PasswordChangeByEmailCommand>
{
    public PasswordChangeByEmailCommandHandlerValidator()
    {
        RuleFor(x => x.Password)
                .NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Password])
                .MinimumLength(8).WithMessage(Strings.DynamicMinLength, [Strings.Password, "8"])
                .Matches("[a-zA-Z]").WithMessage(Strings.PasswordLetter)
                .Matches("[0-9]").WithMessage(Strings.PasswordNumber);

        RuleFor(x => x.Token).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.PasswordCode]);
    }
}