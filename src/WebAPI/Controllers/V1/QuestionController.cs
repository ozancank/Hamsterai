using Application.Features.Questions.Commands.Questions;
using Application.Features.Questions.Commands.Similars;
using Application.Features.Questions.Models;
using Application.Features.Questions.Models.Questions;
using Application.Features.Questions.Models.Similars;
using Application.Features.Questions.Queries.Questions;
using Application.Features.Questions.Queries.Similars;
using Asp.Versioning;

namespace WebAPI.Controllers.V1;

[ApiController]
[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
[ApiVersion("1")]
public class QuestionController() : BaseController
{
    #region Question

    [HttpGet("GetQuestionById/{id}")]
    public async Task<IActionResult> GetQuestionById([FromRoute] Guid id)
    {
        var query = new GetQuestionByIdQuery { Id = id };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("GetQuestions")]
    public async Task<IActionResult> GetQuestions([FromQuery] PageRequest pageRequest, [FromBody] QuestionRequestModel model)
    {
        var query = new GetQuestionsQuery { PageRequest = pageRequest, Model = model };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("AddQuestion")]
    public async Task<IActionResult> AddQuestion([FromBody] AddQuestionModel model)
    {
        var command = new AddQuestionCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("AddRangeQuestion")]
    public async Task<IActionResult> AddRangeQuestion([FromBody] List<AddQuestionModel> models)
    {
        var command = new AddRangeQuestionCommand { Models = models };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("PassiveQuestion")]
    public async Task<IActionResult> PassiveQuestion([FromBody] Guid questionId)
    {
        var command = new PassiveQuestionCommand { QuestionId = questionId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("ActiveQuestion")]
    public async Task<IActionResult> ActiveQuestion([FromBody] Guid questionId)
    {
        var command = new ActiveQuestionCommand { QuestionId = questionId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("IsReadQuestion")]
    public async Task<IActionResult> IsReadQuestion([FromQuery] Guid questionId)
    {
        var command = new UpdateQuestionIsReadCommand { Id = questionId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    #endregion Question

    #region SimilarQuestion

    [HttpGet("GetSimilarQuestionById/{id}")]
    public async Task<IActionResult> GetSimilarQuestionById([FromRoute] Guid id)
    {
        var query = new GetSimilarByIdQuery { Id = id };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("GetSimilarQuestions")]
    public async Task<IActionResult> GetSimilarQuestions([FromQuery] PageRequest pageRequest, [FromBody] SimilarRequestModel model)
    {
        var query = new GetSimilarsQuery { PageRequest = pageRequest, Model = model };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("AddSimilarQuestion")]
    public async Task<IActionResult> AddSimilarQuestion([FromBody] AddSimilarModel model)
    {
        var command = new AddSimilarCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("PassiveSimilarQuestion")]
    public async Task<IActionResult> PassiveSimilarQuestion([FromBody] Guid similarQuestionId)
    {
        var command = new PassiveSimilarCommand { QuestionId = similarQuestionId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("ActiveSimilarQuestion")]
    public async Task<IActionResult> ActiveSimilarQuestion([FromBody] Guid similarQuestionId)
    {
        var command = new ActiveSimilarCommand { QuestionId = similarQuestionId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("IsReadSimilarQuestion")]
    public async Task<IActionResult> IsReadSimilarQuestion([FromBody] Guid similarQuestionId)
    {
        var command = new UpdateSimilarIsReadCommand { Id = similarQuestionId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    #endregion SimilarQuestion

    [HttpPost("AddManuel")]
    public async Task<IActionResult> AddManuel([FromBody] AddManuelModel model)
    {
        var command = new AddManuelCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}