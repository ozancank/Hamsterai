using MediatR;
using OCK.Core.Constants;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Security.Extensions;
using OCK.Core.Security.Headers;

namespace Application.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse>(IHttpContextAccessor httpContextAccessor)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>, ISecuredRequest<UserTypes>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (httpContextAccessor.HttpContext == null) throw new AuthenticationException(Strings.AuthorizationDenied);

        if (ByPassOptions.IsValid)
        {
            httpContextAccessor.HttpContext.Request.Headers.TryGetValue(ByPassOptions.Name, out var byPassKey);
            if (ByPassOptions.Key == byPassKey && request.AllowByPass) return await next();
        }

        var isAuthenticated = httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        if (Delegates.ControlUserStatusAsync != null)
        {
            var id = Convert.ToInt64(httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "-9999");
            if (!await Delegates.ControlUserStatusAsync.Invoke(id)) throw new AuthenticationException(Strings.AuthorizationDenied);
        }

        if (!isAuthenticated) throw new AuthorizationException(Strings.AuthorizationDenied);
        if (isAuthenticated && request.Roles.Length == 0) return await next();

        var roleClaims = httpContextAccessor.HttpContext!.User.ClaimRoles() ?? [];
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