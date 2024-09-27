using Asp.Versioning;
using Business.Features.Students.Commands;
using Business.Features.Students.Models;
using Business.Features.Students.Queries;

namespace WebAPI.Controllers.V1;

[ApiController]
[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
[ApiVersion("1")]
public class StudentController : BaseController
{
    [HttpGet("GetStudentById/{id}")]
    public async Task<IActionResult> GetStudentById([FromRoute] byte id)
    {
        var command = new GetStudentByIdQuery { Id = id };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetStudentGainsForSelf")]
    public async Task<IActionResult> GetStudentGainsForSelf()
    {
        var getStudentGainsForSelfQuery = new GetStudentGainsForSelfQuery();
        var result = await Mediator.Send(getStudentGainsForSelfQuery);
        return Ok(result);
    }

    [HttpGet("GetStudents")]
    public async Task<IActionResult> GetStudents([FromQuery] PageRequest model)
    {
        var command = new GetStudentsQuery { PageRequest = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("GetStudentsDynamic")]
    public async Task<IActionResult> GetStudentsDynamic([FromQuery] PageRequest model, [FromBody] Dynamic dynamicModel)
    {
        var command = new GetStudentsByDynamicQuery { PageRequest = model, Dynamic = dynamicModel };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("AddStudent")]
    public async Task<IActionResult> AddStudent([FromBody] AddStudentModel model)
    {
        var command = new AddStudentCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("UpdateStudent")]
    public async Task<IActionResult> UpdateStudent([FromBody] UpdateStudentModel model)
    {
        var command = new UpdateStudentCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("PassiveStudent")]
    public async Task<IActionResult> PassiveStudent([FromBody] int studentId)
    {
        var command = new PassiveStudentCommand { Id = studentId };
        await Mediator.Send(command);
        return Ok();
    }

    [HttpPost("ActiveStudent")]
    public async Task<IActionResult> ActiveStudent([FromBody] int studentId)
    {
        var command = new ActiveStudentCommand { Id = studentId };
        await Mediator.Send(command);
        return Ok();
    }
}