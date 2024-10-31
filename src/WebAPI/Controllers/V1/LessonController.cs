using Asp.Versioning;
using Business.Features.Lessons.Commands.Lessons;
using Business.Features.Lessons.Models.Lessons;
using Business.Features.Lessons.Queries.Gains;
using Business.Features.Lessons.Queries.Lessons;

namespace WebAPI.Controllers.V1;

[ApiController]
[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
[ApiVersion("1")]
public class LessonController() : BaseController
{
    #region Lesson

    [HttpGet("GetLessonById/{id}")]
    public async Task<IActionResult> GetLessonById([FromRoute] byte id)
    {
        var query = new GetLessonByIdQuery { Id = id };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("GetLessons")]
    public async Task<IActionResult> GetLessons([FromQuery] PageRequest model)
    {
        var query = new GetLessonsQuery { PageRequest = model };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("GetLessonsDynamic")]
    public async Task<IActionResult> GetLessonsDynamic([FromQuery] PageRequest model, [FromBody] Dynamic dynamic)
    {
        var query = new GetLessonsByDynamicQuery { PageRequest = model, Dynamic = dynamic };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("AddLesson")]
    public async Task<IActionResult> AddLesson([FromBody] AddLessonModel model)
    {
        var command = new AddLessonCommand { AddLessonModel = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("UpdateLesson")]
    public async Task<IActionResult> UpdateLesson([FromBody] UpdateLessonModel model)
    {
        var command = new UpdateLessonCommand { UpdateLessonModel = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("PassiveLesson")]
    public async Task<IActionResult> PassiveLesson([FromBody] byte gainId)
    {
        var command = new PassiveLessonCommand { Id = gainId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("ActiveLesson")]
    public async Task<IActionResult> ActiveLesson([FromBody] byte gainId)
    {
        var command = new ActiveLessonCommand { Id = gainId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    #endregion Lesson

    #region Gain

    [HttpGet("GetGainById/{id}")]
    public async Task<IActionResult> GetGainById([FromRoute] byte id)
    {
        var query = new GetGainByIdQuery { Id = id };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("GetGains")]
    public async Task<IActionResult> GetGains([FromQuery] PageRequest model)
    {
        var query = new GetGainsQuery { PageRequest = model };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("GetGainsDynamic")]
    public async Task<IActionResult> GetGainsDynamic([FromQuery] PageRequest model, [FromBody] Dynamic dynamic)
    {
        var query = new GetGainsByDynamicQuery { PageRequest = model, Dynamic = dynamic };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    #endregion Gain
}