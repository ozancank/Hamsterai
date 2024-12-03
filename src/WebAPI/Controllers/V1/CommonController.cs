using Application.Services.CommonService;
using Asp.Versioning;
using Infrastructure.Payment;
using OCK.Core.Utilities;

namespace WebAPI.Controllers.V1;

[ApiController]
[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
[ApiVersion("1")]
public class CommonController(ICommonService commonService, IPaymentApi paymentApi) : BaseController
{
    [HttpPost("ThrowErrorTry")]
    public IActionResult ThrowErrorTry()
    {
        commonService.ThrowErrorTry(new Exception("Bu bir deneme hatasýdýr."));
        return Ok();
    }

    [HttpGet("GetLogs")]
    public IActionResult GetLogs([FromQuery] PageRequest pageRequest, [FromQuery] bool onlyError)
    {
        var result = commonService.GetLogs(pageRequest, onlyError);
        return Ok(result);
    }

    [HttpGet("GetEnums")]
    public IActionResult GetEnums()
    {
        var result = commonService.GetEnums();
        return Ok(result);
    }

    [HttpGet("GetEntities")]
    public IActionResult GetEntities()
    {
        var result = commonService.GetEntities();
        return Ok(result);
    }

    [HttpGet("GetCulture")]
    public IActionResult GetCulture()
    {
        var result = TimeTools.GetCultureInfo();
        return Ok(result);
    }

    [HttpGet("GetLessonNamesForAI")]
    public async Task<IActionResult> GetLessonNamesForAI()
    {
        var result = await commonService.GetLessonNamesForAI();
        return Ok(result);
    }

    [HttpGet("Payment")]
    public async Task<IActionResult> Payment()
    {
        await paymentApi.PaymentControl("422c196f-391f-4406-8cab-5641f50abfb4", 2);
        return Ok();
    }
}