using Application.Features.Auths.Models;
using Application.Features.Auths.Rules;
using Application.Features.Users.Models.User;
using Application.Services.AuthService;
using DataAccess.Abstract.Core;
using DataAccess.EF;
using Domain.Entities.Core;
using MediatR;
using System.Linq.Expressions;

namespace Application.Features.Auths.Commands.Logins;

public class LoginCommand : IRequest<TokenModel>
{
    public required LoginModel LoginModel { get; set; }
    public required string IpAddress { get; set; }
    public bool WebLogin { get; set; } = false;
}

public class LoginCommandHandler(IUserDal userDal,
                                 IAuthService authService,
                                 IMapper mapper) : IRequestHandler<LoginCommand, TokenModel>
{
    public async Task<TokenModel> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        Expression<Func<User, bool>> predicate = request.WebLogin
            ? x => PostgresqlFunctions.TrLower(x.UserName) == PostgresqlFunctions.TrLower(request.LoginModel.UserName) && x.Type != UserTypes.Student
            : x => PostgresqlFunctions.TrLower(x.UserName) == PostgresqlFunctions.TrLower(request.LoginModel.UserName);

        var user = await userDal.GetAsync(
            predicate: predicate,
            include: x => x.Include(u => u.UserOperationClaims).ThenInclude(u => u.OperationClaim!)
                           .Include(u => u.PackageUsers).ThenInclude(u => u.Package)
                           .Include(u => u.Questions)
                           .Include(u => u.RefreshTokens),
            cancellationToken: cancellationToken);

        await AuthRules.UserShouldExistsByUserNameWhenLogin(user);
        await AuthRules.PasswordShouldVerifiedWhenLogin(user, request.LoginModel.Password!);
        await AuthRules.UserShouldExistsAndActiveByUserNameWhenLogin(user);

        var createdAccessToken = await authService.CreateAccessToken(user);
        //var createdRefreshToken = await authService.CreateRefreshToken(user, request.IpAddress!);
        //var addedRefreshToken = await authService.AddRefreshToken(createdRefreshToken);
        //await authService.DeleteOldRefreshTokens(user.Id);

        var tokenModel = new TokenModel
        {
            AccessToken = mapper.Map<AccessTokenModel>(createdAccessToken),
            RefreshToken = new RefreshTokenModel { Token = string.Empty, Expiration = DateTime.Now },
            UserInfo = mapper.Map<GetUserModel>(user)
        };
        return tokenModel;
    }
}

public class LoginCommandHandlerValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandHandlerValidator()
    {
        RuleFor(x => x).NotNull().WithMessage(Strings.UserOrPasswordNotWrong);

        RuleFor(x => x.LoginModel).NotNull().WithMessage(Strings.UserOrPasswordNotWrong);

        RuleFor(x => x.LoginModel!.UserName).NotEmpty().WithMessage(Strings.UserOrPasswordNotWrong);

        RuleFor(x => x.LoginModel!.Password).NotEmpty().WithMessage(Strings.UserOrPasswordNotWrong);
    }
}