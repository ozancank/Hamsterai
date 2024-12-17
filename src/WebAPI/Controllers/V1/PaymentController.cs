using Application.Features.Payments.Commands;
using Application.Features.Payments.Queries;
using Asp.Versioning;
using Infrastructure.Payment.Sipay.Models;
using OCK.Core.Extensions;
using OCK.Core.Logging.Serilog;
using OCK.Core.Utilities;
using System.Text.Json;

namespace WebAPI.Controllers.V1;

[ApiController]
[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
[ApiVersion("1")]
public class PaymentController(LoggerServiceBase loggerServiceBase) : BaseController
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

    [HttpPost("AddPaymentSipay")]
    [Consumes("application/json", "application/x-www-form-urlencoded", "multipart/form-data", "text/plain")]
    public async Task<IActionResult> AddPaymentSipay()
    {
        var methodName = nameof(AddPaymentSipay);
        var contentType = HttpContext.Request.ContentType;
        Console.WriteLine($"{methodName} ----- {contentType}");

        async Task<IActionResult> ProcessRequest(SipayRecurringWebHookRequestModel model)
        {
            if (model != null && model.PlanCode.IsNotEmpty())
            {
                var command = new AddPaymentSipayCommand { Model = model };
                await Mediator.Send(command);
                return Ok();
            }

            loggerServiceBase.Error($"{methodName} ----- Error ----- PlanCode is required.");
            return BadRequest();
        }

        try
        {
            if (contentType.Contains("application/json") || contentType.Contains("text/plain"))
            {
                using var reader = new StreamReader(Request.Body);
                var jsonString = await reader.ReadToEndAsync();
                loggerServiceBase.Info($"{methodName} ----- {contentType} ----- {jsonString}");
                Console.WriteLine($"{methodName} ----- {contentType} ----- {jsonString}");
                var model = JsonSerializer.Deserialize<SipayRecurringWebHookRequestModel>(jsonString);

                return await ProcessRequest(model);
            }

            if (contentType.Contains("application/x-www-form-urlencoded"))
            {
                var form = await Request.ReadFormAsync();
                var model = new SipayRecurringWebHookRequestModel();
                var formDataString = string.Join("&", form.Keys.Select(key => $"{key}={form[key]}"));
                loggerServiceBase.Info($"{methodName} ----- {contentType} ----- {formDataString}");
                Console.WriteLine($"{methodName} ----- {contentType} ----- {formDataString}");
                foreach (var key in form.Keys)
                    ReflectionTools.SetPropertyValue(model, key, form[key]);

                return await ProcessRequest(model);
            }

            if (contentType.Contains("multipart/form-data"))
            {
                var form = Request.Form;
                var model = new SipayRecurringWebHookRequestModel();
                var formDataString = string.Join("&", form.Keys.Select(key => $"{key}={form[key]}"));
                loggerServiceBase.Info($"{methodName} ----- {contentType} ----- {formDataString}");
                Console.WriteLine($"{methodName} ----- {contentType} ----- {formDataString}");
                foreach (var key in form.Keys)
                    ReflectionTools.SetPropertyValue(model, key, form[key]);

                return await ProcessRequest(model);
            }

            throw new Exception($"{methodName} ----- Error ----- {contentType}");
        }
        catch (Exception ex)
        {
            loggerServiceBase.Error($"{methodName} ----- {ex.Message}");
            return BadRequest();
        }
    }
}