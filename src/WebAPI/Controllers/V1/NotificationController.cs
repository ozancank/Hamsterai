using Asp.Versioning;
using Business.Features.Notifications.Commands.DeviceToken;
using Business.Features.Notifications.Commands.Notifications;
using Business.Features.Notifications.Models.DeviceToken;
using Business.Features.Notifications.Models.Notification;
using Business.Features.Notifications.Queries;

namespace WebAPI.Controllers.V1;

[ApiController]
[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
[ApiVersion("1")]
public class NotificationController : BaseController
{
    [HttpPost("PushNotificationByUserId")]
    public async Task<IActionResult> PushNotificationByUserId([FromBody] MessageRequestModel request, [FromQuery] long userId)
    {
        var command = new PushNotificationByUserIdCommand { Model = request, UserId = userId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

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

    [HttpGet("GetNotificationsByUserId")]
    public async Task<IActionResult> GetNotificationsByUserId([FromQuery] PageRequest pageRequest)
    {
        var query = new GetNotificationsQuery { PageRequest = pageRequest };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("GetNotificationsByUserIdDynamic")]
    public async Task<IActionResult> GetNotificationsByUserIdDynamic([FromQuery] PageRequest pageRequest, [FromBody] Dynamic dynamic)
    {
        var queryModel = new GetNotificationsDynamicQuery { PageRequest = pageRequest, Dynamic = dynamic };
        var result = await Mediator.Send(queryModel);
        return Ok(result);
    }

    [HttpPost("ReadNotification")]
    public async Task<IActionResult> ReadNotification([FromQuery] Guid id)
    {
        var command = new ReadNotificationCommand { Id = id };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("ReadAllNotification")]
    public async Task<IActionResult> ReadAllNotification()
    {
        var command = new ReadAllNotificationCommand();
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("UnreadNotification")]
    public async Task<IActionResult> UnreadNotification([FromQuery] Guid id)
    {
        var command = new UnreadNotificationCommand { Id = id };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("PassiveNotification")]
    public async Task<IActionResult> PassiveNotification([FromQuery] Guid id)
    {
        var command = new PassiveNotificationCommand { Id = id };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("PassiveAllNotification")]
    public async Task<IActionResult> PassiveAllNotification()
    {
        var command = new PassiveAllNotificationCommand();
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}