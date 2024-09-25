using Asp.Versioning;
using Business.Features.Schools.Queries.Schools;

namespace WebAPI.Controllers.V1;

[ApiController]
[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
[ApiVersion("1")]
public class SchoolController : BaseController
{
    #region School

    [HttpGet("GetSchoolById/{id}")]
    public async Task<IActionResult> GetSchoolById([FromRoute] byte id)
    {
        var command = new GetSchoolByIdQuery { Id = id };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetSchoolByTaxNumber/{taxNumber}")]
    public async Task<IActionResult> GetSchoolByTaxNumber(string taxNumber)
    {
        var command = new GetSchoolByTaxNumberQuery { TaxNumber = taxNumber };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetSchoolForDashboard")]
    public async Task<IActionResult> GetSchoolForDashboard()
    {
        var command = new GetSchoolDashboardQuery();
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetSchools")]
    public async Task<IActionResult> GetSchools([FromQuery] PageRequest model)
    {
        var command = new GetSchoolsQuery { PageRequest = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("GetSchoolsDynamic")]
    public async Task<IActionResult> GetSchoolsDynamic([FromQuery] PageRequest model, [FromBody] Dynamic dynamicModel)
    {
        var command = new GetSchoolsByDynamicQuery { PageRequest = model, Dynamic = dynamicModel };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    //[HttpPost("AddSchool")]
    //public async Task<IActionResult> AddSchool([FromBody] AddSchoolModel model)
    //{
    //    var result = await SchoolService.AddSchool(model);
    //    return Ok(result);
    //}

    //[HttpPost("UpdateSchool")]
    //public async Task<IActionResult> UpdateSchool([FromBody] UpdateSchoolModel model)
    //{
    //    var result = await SchoolService.UpdateSchool(model);
    //    return Ok(result);
    //}

    //[HttpPost("PassiveSchool")]
    //public async Task<IActionResult> PassiveSchool([FromBody] int SchoolId)
    //{
    //    await SchoolService.PassiveSchool(SchoolId);
    //    return Ok();
    //}

    //[HttpPost("ActiveSchool")]
    //public async Task<IActionResult> ActiveSchool([FromBody] int SchoolId)
    //{
    //    await SchoolService.ActiveSchool(SchoolId);
    //    return Ok();
    //}

    #endregion School
}