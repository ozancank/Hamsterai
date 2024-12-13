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
        commonService.ThrowErrorTry(new Exception("Bu bir deneme hatasıdır."));
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
        var result = await paymentApi.GetPayment("0d2a2735-55c7-49cc-b7d7-88264f8d37f1", 1);
        return Ok(result);
    }
}