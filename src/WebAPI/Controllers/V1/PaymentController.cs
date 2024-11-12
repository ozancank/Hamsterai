using Application.Features.Payments.Queries;
using Asp.Versioning;

namespace WebAPI.Controllers.V1;

[ApiController]
[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
[ApiVersion("1")]
public class PaymentController : BaseController
{
    [HttpGet("GetPaymentById/{id}")]
    public async Task<IActionResult> GetPaymentById([FromRoute] Guid id)
    {
        var command = new GetPaymentByIdQuery { Id = id };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetPayments")]
    public async Task<IActionResult> GetPayments([FromQuery] PageRequest model)
    {
        var command = new GetPaymentsQuery { PageRequest = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetPaymentsByUserId/{userId}")]
    public async Task<IActionResult> GetPaymentsByUserId([FromRoute] long userId, [FromQuery] PageRequest model)
    {
        var command = new GetPaymentsByUserIdQuery { PageRequest = model, UserId = userId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("GetPaymentsDynamic")]
    public async Task<IActionResult> GetPaymentsDynamic([FromQuery] PageRequest model, [FromBody] Dynamic dynamicModel)
    {
        var command = new GetPaymentsByDynamicQuery { PageRequest = model, Dynamic = dynamicModel };
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}