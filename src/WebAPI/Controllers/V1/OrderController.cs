using Application.Features.Orders.Commands;
using Application.Features.Orders.Models;
using Application.Features.Orders.Queries;
using Asp.Versioning;

namespace WebAPI.Controllers.V1;

[ApiController]
[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
[ApiVersion("1")]
public class OrderController : BaseController
{
    [HttpGet("GetOrderById/{id}")]
    public async Task<IActionResult> GetOrderById([FromRoute] int id)
    {
        var command = new GetOrderByIdQuery { OrderId = id };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetOrderDetailsByOrderId/{orderId}")]
    public async Task<IActionResult> GetOrderDetailsByOrderId([FromRoute] int orderId)
    {
        var command = new GetOrderDetailsByOrderIdQuery { OrderId = orderId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetOrders")]
    public async Task<IActionResult> GetOrders([FromQuery] PageRequest model)
    {
        var command = new ControlExitPassQuery { PageRequest = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetOrdersByUserId/{userId}")]
    public async Task<IActionResult> GetOrdersByUserId([FromRoute] long userId, [FromQuery] PageRequest model)
    {
        var command = new GetOrdersByUserIdQuery { PageRequest = model, UserId = userId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("GetOrdersDynamic")]
    public async Task<IActionResult> GetOrdersDynamic([FromQuery] PageRequest model, [FromBody] Dynamic dynamicModel)
    {
        var command = new GetOrdersByDynamicQuery { PageRequest = model, Dynamic = dynamicModel };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("PreControlForAddOrder")]
    public async Task<IActionResult> PreControlForAddOrder([FromBody] AddOrderModel model)
    {
        var command = new AddOrderCommand { Model = model, WillSave = false };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("AddOrder")]
    public async Task<IActionResult> AddOrder([FromBody] AddOrderModel model)
    {
        var command = new AddOrderCommand { Model = model, WillSave = true };
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