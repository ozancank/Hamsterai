using Asp.Versioning;
using Business.Features.Notifications.Commands.DeviceToken;
using Business.Features.Notifications.Commands.Notification;
using Business.Features.Notifications.Models.DeviceToken;
using Business.Features.Notifications.Models.Notification;

namespace WebAPI.Controllers.V1;

[ApiController]
[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
[ApiVersion("1")]
public class NotificationController : BaseController
{
    [HttpPost("PushNotificationAll")]
    public async Task<IActionResult> PushNotificationAll([FromBody] MessageRequestModel request)
    {
        var command = new PushNotificationAllCommand { Model = request };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("AddDeviceToken")]
    public async Task<IActionResult> AddDeviceToken([FromBody] DeviceTokenModel model)
    {
        var command = new AddDeviceTokenCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("PassiveDeviceToken")]
    public async Task<IActionResult> PassiveDeviceToken([FromQuery] long userId)
    {
        var command = new PassiveDeviceTokenCommand { UserId = userId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("ActiveDeviceToken")]
    public async Task<IActionResult> ActiveDeviceToken([FromQuery] long userId)
    {
        var command = new ActiveDeviceTokenCommand { UserId = userId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}