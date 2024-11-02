using Asp.Versioning;
using Business.Services.CommonService;

namespace WebAPI.Controllers.V1;

[ApiController]
[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
[ApiVersion("1")]
public class CommonController(ICommonService commonService) : BaseController
{
    [HttpGet("GetEnums")]
    public IActionResult MikroTypes()
    {
        var result = commonService.GetEnums();
        return Ok(result);
    }

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
}