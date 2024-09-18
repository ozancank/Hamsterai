using Asp.Versioning;
using Business.Features.Auths.Commands.Logins;
using Business.Features.Auths.Commands.RefreshTokens;
using Business.Features.Auths.Models;
using OCK.Core.Security.JWT;

namespace WebAPI.Controllers.V1;

[ApiController]
[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
[ApiVersion("1")]
public class AuthController(IConfiguration configuration) : BaseController
{
    private readonly TokenCookieOptions _tokenCookieOptions = configuration.GetSection("TokenCookieOptions").Get<TokenCookieOptions>();
    private readonly TokenOptions _tokenOptions = configuration.GetSection("TokenOptions").Get<TokenOptions>();

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        var loginCommand = new LoginCommand() { LoginModel = loginModel, IpAddress = GetIpAddress(), WebLogin = false };
        var result = await Mediator.Send(loginCommand);
        SetTokensToCookie(result.AccessToken, result.RefreshToken);
        return Ok(result);
    }

    [HttpPost("LoginForWeb")]
    public async Task<IActionResult> LoginForWeb([FromBody] LoginModel loginModel)
    {
        var loginCommand = new LoginCommand() { LoginModel = loginModel, IpAddress = GetIpAddress(), WebLogin = true };
        var result = await Mediator.Send(loginCommand);
        SetTokensToCookie(result.AccessToken, result.RefreshToken);
        return Ok(result);
    }

    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshTokenCommand = new RefreshTokenCommand() { RefreshToken = GetRefreshTokenFromCookies(), IpAddress = GetIpAddress() };
        var result = await Mediator.Send(refreshTokenCommand);
        if (result.AccessToken == null || result.RefreshToken == null) return BadRequest();
        SetTokensToCookie(result.AccessToken, result.RefreshToken);
        return Ok(result.AccessToken);
    }

    [HttpGet("Logout")]
    public async Task<IActionResult> Logout()
    {
        var revokeTokenCommand = new RevokeTokenCommand() { RefreshToken = GetRefreshTokenFromCookies(), IpAddress = GetIpAddress() };
        await Mediator.Send(revokeTokenCommand);
        if (Request.Cookies.ContainsKey(_tokenCookieOptions.AccessTokenCookieName!)) Response.Cookies.Delete(_tokenCookieOptions.AccessTokenCookieName!);
        if (Request.Cookies.ContainsKey(_tokenCookieOptions.RefreshTokenCookieName!)) Response.Cookies.Delete(_tokenCookieOptions.RefreshTokenCookieName!);
        return Ok();
    }
    private string GetRefreshTokenFromCookies() => Request.Cookies[_tokenCookieOptions.RefreshTokenCookieName];

    private void SetTokensToCookie(AccessTokenModel accessToken, RefreshTokenModel refreshToken)
    {
        Response.Cookies.Append(_tokenCookieOptions.AccessTokenCookieName, accessToken.Token, new CookieOptions()
        { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict, Expires = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration) });
        Response.Cookies.Append(_tokenCookieOptions.RefreshTokenCookieName, refreshToken.Token, new CookieOptions()
        { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict, Expires = DateTime.Now.AddDays(7) });
    }
}