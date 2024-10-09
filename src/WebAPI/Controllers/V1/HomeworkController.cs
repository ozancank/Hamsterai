using Asp.Versioning;
using Business.Features.Homeworks.Commands;
using Business.Features.Homeworks.Models;
using Business.Features.Homeworks.Queries;

namespace WebAPI.Controllers.V1;

[ApiController]
[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
[ApiVersion("1")]
public class HomeworkController : BaseController
{
    [HttpGet("GetHomeworkById/{id}")]
    public async Task<IActionResult> GetHomeworkById([FromRoute] string id)
    {
        var command = new GetHomeworkByIdQuery { Id = id };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetHomeworkForStudentById/{id}")]
    public async Task<IActionResult> GetHomeworkForStudentById([FromRoute] string id)
    {
        var command = new GetHomeworkForStudentByIdQuery { Id = id };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("GetHomeworks")]
    public async Task<IActionResult> GetHomeworksDynamic([FromQuery] PageRequest pageRequest, [FromBody] HomeworkRequestModel model)
    {
        var command = new GetHomeworksQuery { PageRequest = pageRequest, Model=model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetHomeworksForStudent")]
    public async Task<IActionResult> GetHomeworksForStudent([FromQuery] PageRequest pageRequest, [FromBody] HomeworkRequestModel model)
    {
        var command = new GetHomeworksForStudentQuery { PageRequest = pageRequest, Model= model};
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("GetHomeworksDynamic")]
    public async Task<IActionResult> GetHomeworksDynamic([FromQuery] PageRequest pageRequest, [FromBody] Dynamic dynamicModel)
    {
        var command = new GetHomeworksByDynamicQuery { PageRequest = pageRequest, Dynamic = dynamicModel };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("AddHomework")]
    public async Task<IActionResult> AddHomework([FromForm] AddHomeworkModel model)
    {
        var command = new AddHomeworkCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("UpdateHomeworkRead")]
    public async Task<IActionResult> UpdateHomeworkRead([FromQuery] string homeworkStudentId)
    {
        var command = new UpdateHomeworkReadCommand { HomeworkStudentId = homeworkStudentId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("UpdateHomeworkAnswer")]
    public async Task<IActionResult> UpdateHomeworkAnswer([FromQuery] HomeworkAnswerRequestModel model)
    {
        var command = new UpdateHomeworkAnswerCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("PassiveHomework")]
    public async Task<IActionResult> PassiveHomework([FromBody] string homeworkId)
    {
        var command = new PassiveHomeworkCommand { HomeworkId = homeworkId };
        await Mediator.Send(command);
        return Ok();
    }

    [HttpPost("ActiveHomework")]
    public async Task<IActionResult> ActiveHomework([FromBody] string homeworkId)
    {
        var command = new ActiveHomeworkCommand { HomeworkId = homeworkId };
        await Mediator.Send(command);
        return Ok();
    }
}