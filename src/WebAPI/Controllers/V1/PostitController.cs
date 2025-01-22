using Application.Features.Postits.Commands;
using Application.Features.Postits.Models;
using Application.Features.Postits.Queries;
using Asp.Versioning;

namespace WebAPI.Controllers.V1;

[ApiController]
[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
[ApiVersion("1")]
public class PostitController : BaseController
{
    #region Postit

    [HttpGet("GetPostitPicture/{postitId}")]
    public async Task<IActionResult> GetPostitPicture([FromRoute] Guid postitId)
    {
        var query = new GetPostitPictureQuery { PostitId = postitId };
        var result = await Mediator.Send(query);
        return result != null ? File(result, "image/jpeg") : NotFound();
    }

    [HttpGet("GetPostitById/{id}")]
    public async Task<IActionResult> GetPostitById([FromRoute] Guid id)
    {
        var query = new GetPostitByIdQuery { Id = id };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("GetPostits")]
    public async Task<IActionResult> GetPostits([FromQuery] PageRequest pageRequest, [FromBody] PostitRequestModel model)
    {
        var query = new GetPostitsQuery { PageRequest = pageRequest, Model = model };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("GetPostitsDynamic")]
    public async Task<IActionResult> GetPostitsDynamic([FromQuery] PageRequest pageRequest, [FromBody] Dynamic dynamicModel)
    {
        var query = new GetPostitsByDynamicQuery { PageRequest = pageRequest, Dynamic = dynamicModel };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("AddPostit")]
    public async Task<IActionResult> AddPostit([FromBody] AddPostitModel model)
    {
        var command = new AddPostitCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("UpdatePostit")]
    public async Task<IActionResult> UpdatePostit([FromBody] UpdatePostitModel model)
    {
        var command = new UpdatePostitCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("Delete")]
    public async Task<IActionResult> DeletePostit([FromQuery] Guid id)
    {
        var command = new DeletePostitCommand { Id = id };
        await Mediator.Send(command);
        return Ok();
    }

    #endregion Postit
}