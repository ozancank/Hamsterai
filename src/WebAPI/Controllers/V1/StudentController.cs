using Asp.Versioning;
using Business.Features.Students.Query;

namespace WebAPI.Controllers.V1;

[ApiController]
[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
[ApiVersion("1")]
public class StudentController : BaseController
{
    [HttpGet("GetStudentGainsForSelf")]
    public async Task<IActionResult> GetStudentGainsForSelf()
    {
        var getStudentGainsForSelfQuery = new GetStudentGainsForSelfQuery();
        var result = await Mediator.Send(getStudentGainsForSelfQuery);
        return Ok(result);
    }
}