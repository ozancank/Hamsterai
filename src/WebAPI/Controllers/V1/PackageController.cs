using Asp.Versioning;
using Business.Features.Packages.Commands;
using Business.Features.Packages.Models;
using Business.Features.Packages.Queries;

namespace WebAPI.Controllers.V1;

[ApiController]
[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
[ApiVersion("1")]
public class PackageController() : BaseController
{
    [HttpGet("GetPackageById/{id}")]
    public async Task<IActionResult> GetPackageById([FromRoute] byte id)
    {
        var query = new GetPackageByIdQuery { Id = id };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("GetPackages")]
    public async Task<IActionResult> GetPackages([FromQuery] PageRequest model)
    {
        var query = new GetPackagesQuery { PageRequest = model };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("GetPackagesDynamic")]
    public async Task<IActionResult> GetPackagesDynamic([FromQuery] PageRequest model, [FromBody] Dynamic dynamic)
    {
        var query = new GetPackagesByDynamicQuery { PageRequest = model, Dynamic = dynamic };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("AddPackage")]
    public async Task<IActionResult> AddPackage([FromBody] AddPackageModel model)
    {
        var command = new AddPackageCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("AddLessonInPackage")]
    public async Task<IActionResult> AddLessonInPackage([FromBody] AddLessonInPackageModel model)
    {
        var command = new AddLessonInPackageCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("UpdatePackage")]
    public async Task<IActionResult> UpdatePackage([FromBody] UpdatePackageModel model)
    {
        var command = new UpdatePackageCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("PassivePackage")]
    public async Task<IActionResult> PassivePackage([FromBody] byte packageId)
    {
        var command = new PassivePackageCommand { Id = packageId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("ActivePackage")]
    public async Task<IActionResult> ActivePackage([FromBody] byte packageId)
    {
        var command = new ActivePackageCommand { Id = packageId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}