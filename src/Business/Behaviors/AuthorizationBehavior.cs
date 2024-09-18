using MediatR;
using OCK.Core.Constants;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Security.Extensions;

namespace Business.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse>(IHttpContextAccessor httpContextAccessor,
                                                        IConfiguration configuration)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>, ISecuredRequest<UserTypes>
{
    private readonly string _byPassName = configuration.GetSection("ByPassOptions:Name")?.Value;
    private readonly string _byPassKey = configuration.GetSection("ByPassOptions:Key")?.Value;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_byPassName) && !string.IsNullOrWhiteSpace(_byPassKey))
        {
            httpContextAccessor.HttpContext.Request.Headers.TryGetValue(_byPassName, out var byPassKey);
            if (_byPassKey == byPassKey) return await next();
        }

        var isAuthenticated = httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        if (Delegates.ControlUserStatus != null)
        {
            var id = Convert.ToInt64(httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "-9999");
            if (!(await Delegates.ControlUserStatus.Invoke(id))) throw new AuthenticationException(Strings.AuthorizationDenied);
        }

        if (!isAuthenticated) throw new AuthorizationException(Strings.AuthorizationDenied);
        if (isAuthenticated && request.Roles.Length == 0) return await next();

        var roleClaims = httpContextAccessor.HttpContext.User.ClaimRoles() ?? [];
        if (roleClaims.Contains("Admin")) return await next();

        _ = Enum.TryParse(httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.UserType)?.Value
            ?? "4", out UserTypes userType);
        if (userType == UserTypes.Administator) return await next();

        bool isMatchedAUserTypeWithRequestUserTypes = request.Roles.Any(role => role == userType);
        if (!isMatchedAUserTypeWithRequestUserTypes) throw new AuthorizationException(Strings.AuthorizationDenied);

        var response = await next();
        return response;
    }
}