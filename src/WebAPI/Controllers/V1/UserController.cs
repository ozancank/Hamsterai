using Asp.Versioning;
using Business.Features.Users.Commands.Claim;
using Business.Features.Users.Commands.Password;
using Business.Features.Users.Commands.Users;
using Business.Features.Users.Models.Claim;
using Business.Features.Users.Models.Password;
using Business.Features.Users.Models.User;
using Business.Features.Users.Queries.Claims;
using Business.Features.Users.Queries.Users;

namespace WebAPI.Controllers.V1;

[ApiController]
[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
[ApiVersion("1")]
public class UserController() : BaseController
{
    [HttpGet("GetUserById/{id}")]
    public async Task<IActionResult> GetUserById([FromRoute] long id)
    {
        var getUserByIdQuery = new GetUserByIdQuery { Id = id };
        var result = await Mediator.Send(getUserByIdQuery);
        return Ok(result);
    }

    [HttpGet("GetUsers")]
    public async Task<IActionResult> GetUsers([FromQuery] PageRequest model)
    {
        var getUsersQuery = new GetUsersQuery { PageRequest = model };
        var result = await Mediator.Send(getUsersQuery);
        return Ok(result);
    }

    [HttpPost("GetUsersDynamic")]
    public async Task<IActionResult> GetUsersDynamic([FromQuery] PageRequest model, [FromBody] Dynamic dynamicModel)
    {
        var usersQuery = new GetUsersByDynamicQuery { PageRequest = model, Dynamic = dynamicModel };
        var result = await Mediator.Send(usersQuery);
        return Ok(result);
    }

    [HttpGet("GetPassiveUsers")]
    public async Task<IActionResult> GetPassiveUsers([FromQuery] PageRequest model)
    {
        var getPassiveUsersQuery = new GetPassiveUsersQuery { PageRequest = model };
        var result = await Mediator.Send(getPassiveUsersQuery);
        return Ok(result);
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] AddUserModel addUser)
    {
        var command = new AddUserCommand { AddLessonModel = addUser };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("UpdateUser")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel model)
    {
        var command = new UpdateUserCommand { UpdateUserModel = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("PasswordChangeByManager")]
    public async Task<IActionResult> PasswordChangeByManager([FromBody] UpdateUserPasswordModel model)
    {
        var command = new PasswordChangeByManagerCommand { Id = model.Id, Password = model.Password };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("PasswordChangeByUser")]
    public async Task<IActionResult> PasswordChangeByUser([FromBody] UpdateUserPasswordModel userPasswordDto)
    {
        var command = new PasswordChangeByUserCommand { Password = userPasswordDto.Password };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("PassiveUser")]
    public async Task<IActionResult> PassiveUser([FromQuery] long id)
    {
        var command = new PassiveUserCommand { Id = id };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("ActiveUser")]
    public async Task<IActionResult> ActiveUser([FromQuery] long id)
    {
        var command = new ActiveUserCommand { Id = id };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetOperationClaims")]
    public async Task<IActionResult> GetOperationClaims()
    {
        var query = new GetOperationClaimsQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("UserAssignClaims")]
    public async Task<IActionResult> AssignClaims(AddUserClaimModel model)
    {
        var command = new UserAssignClaimsCommand { Id = model.Id, AssignRoles = [.. model.Roles] };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("UserSendForgetPasswordEmail")]
    public async Task<IActionResult> UserSendForgetPasswordEmail([FromBody] ForgetPasswordModel model)
    {
        var command = new UserSendForgetPasswordEmailCommand { Email = model.Email };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("PasswordChangeByEmail")]
    public async Task<IActionResult> PasswordChangeByEmail(UpdateUserPasswordEmailModel model)
    {
        var command = new PasswordChangeByEmailCommand { Password = model.Password, Token = model.Token };
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}