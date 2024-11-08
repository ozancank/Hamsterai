using Asp.Versioning;
using Business.Features.Orders.Commands;
using Business.Features.Orders.Models;

namespace WebAPI.Controllers.V1;

[ApiController]
[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
[ApiVersion("1")]
public class OrderController : BaseController
{
    //[HttpGet("GetOrderById/{id}")]
    //public async Task<IActionResult> GetOrderById([FromRoute] int id)
    //{
    //    var command = new GetOrderByIdQuery { Id = id };
    //    var result = await Mediator.Send(command);
    //    return Ok(result);
    //}

    //[HttpGet("GetOrderGainsForSelf")]
    //public async Task<IActionResult> GetOrderGainsForSelf()
    //{
    //    var getOrderGainsForSelfQuery = new GetOrderGainsForSelfQuery();
    //    var result = await Mediator.Send(getOrderGainsForSelfQuery);
    //    return Ok(result);
    //}

    //[HttpPost("GetOrderGains")]
    //public async Task<IActionResult> GetOrderGainsForSelf([FromBody] OrderGainsRequestModel model)
    //{
    //    var getOrderGainsByIdQuery = new GetOrderGainsByIdQuery { Model = model };
    //    var result = await Mediator.Send(getOrderGainsByIdQuery);
    //    return Ok(result);
    //}

    //[HttpGet("GetOrders")]
    //public async Task<IActionResult> GetOrders([FromQuery] PageRequest model)
    //{
    //    var command = new GetOrdersQuery { PageRequest = model };
    //    var result = await Mediator.Send(command);
    //    return Ok(result);
    //}

    //[HttpPost("GetOrdersDynamic")]
    //public async Task<IActionResult> GetOrdersDynamic([FromQuery] PageRequest model, [FromBody] Dynamic dynamicModel)
    //{
    //    var command = new GetOrdersByDynamicQuery { PageRequest = model, Dynamic = dynamicModel };
    //    var result = await Mediator.Send(command);
    //    return Ok(result);
    //}

    [HttpPost("AddOrder")]
    public async Task<IActionResult> AddOrder([FromBody] AddOrderModel model)
    {
        var command = new AddOrderCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    //[HttpPost("UpdateOrder")]
    //public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderModel model)
    //{
    //    var command = new UpdateOrderCommand { Model = model };
    //    var result = await Mediator.Send(command);
    //    return Ok(result);
    //}

    //[HttpPost("PassiveOrder")]
    //public async Task<IActionResult> PassiveOrder([FromBody] int orderId)
    //{
    //    var command = new PassiveOrderCommand { Id = orderId };
    //    await Mediator.Send(command);
    //    return Ok();
    //}

    //[HttpPost("ActiveOrder")]
    //public async Task<IActionResult> ActiveOrder([FromBody] int orderId)
    //{
    //    var command = new ActiveOrderCommand { Id = orderId };
    //    await Mediator.Send(command);
    //    return Ok();
    //}
}