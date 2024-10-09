using Asp.Versioning;
using Business.Features.Questions.Commands.Quizzes;
using Business.Features.Questions.Models.Quizzes;
using Business.Features.Questions.Queries.Quizzes;

namespace WebAPI.Controllers.V1;

[ApiController]
[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
[ApiVersion("1")]
public class QuizController : BaseController
{
    [HttpGet("GetQuizById/{quizId}")]
    public async Task<IActionResult> GetQuizById([FromRoute] string quizId)
    {
        var command = new GetQuizByIdQuery { Id = quizId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("GetQuizzes")]
    public async Task<IActionResult> GetQuizzes([FromQuery] PageRequest pageRequest, [FromBody] QuizRequestModel model)
    {
        var command = new GetQuizzesQuery { PageRequest = pageRequest, Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("AddQuiz")]
    public async Task<IActionResult> AddQuiz([FromBody] AddQuizModel model)
    {
        var command = new AddQuizCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("AddQuizForAll")]
    public async Task<IActionResult> AddQuizForAll()
    {
        var command = new AddQuizForAllCommand();
        await Mediator.Send(command);
        return Ok();
    }

    [HttpPost("UpdateQuiz")]
    public async Task<IActionResult> UpdateQuiz([FromBody] UpdateQuizModel model)
    {
        var command = new UpdateQuizCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("UpdateQuizStarted")]
    public async Task<IActionResult> UpdateQuizStarted([FromQuery] string quizId)
    {
        var command = new UpdateQuizStartedCommand { Id = quizId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}