//using Asp.Versioning;
//using Models.Common;
//using Models.Models.Question;

//namespace WebAPI.Controllers.V1;

//[ApiController]
//[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
//[ApiVersion("1")]
//public class QuestionController(IQuestionService questionService) : BaseController
//{
//    #region Question

//    [HttpGet("GetQuestionById/{id}")]
//    public async Task<IActionResult> GetQuestionById([FromRoute] Guid id)
//    {
//        var result = await questionService.GetQuestionById(id);
//        return Ok(result);
//    }

//    [HttpPost("GetQuestions")]
//    public async Task<IActionResult> GetQuestions([FromQuery] PageRequestModel model, [FromBody] QuestionRequestModel questionRequestModel)
//    {
//        var result = await questionService.GetQuestions(model, questionRequestModel);
//        return Ok(result);
//    }

//    [HttpPost("AddQuestion")]
//    public async Task<IActionResult> AddQuestion([FromBody] AddQuestionModel model)
//    {
//        var result = await questionService.AddQuestion(model);
//        return Ok(result);
//    }

//    [HttpPost("PassiveQuestion")]
//    public async Task<IActionResult> PassiveQuestion([FromBody] Guid questionId)
//    {
//        var result = await questionService.PassiveQuestion(questionId);
//        return Ok(result);
//    }

//    [HttpPost("ActiveQuestion")]
//    public async Task<IActionResult> ActiveQuestion([FromBody] Guid questionId)
//    {
//        var result = await questionService.ActiveQuestion(questionId);
//        return Ok(result);
//    }

//    #endregion Question

//    #region SimilarQuestion

//    [HttpGet("GetSimilarQuestionById/{id}")]
//    public async Task<IActionResult> GetSimilarQuestionById([FromRoute] Guid id)
//    {
//        var result = await questionService.GetSimilarQuestionById(id);
//        return Ok(result);
//    }

//    [HttpPost("GetSimilarQuestions")]
//    public async Task<IActionResult> GetSimilarQuestions([FromQuery] PageRequestModel model, [FromBody] SimilarQuestionRequestModel similarQuestionRequestModel)
//    {
//        var result = await questionService.GetSimilarQuestions(model, similarQuestionRequestModel);
//        return Ok(result);
//    }

//    [HttpPost("AddSimilarQuestion")]
//    public async Task<IActionResult> AddSimilarQuestion([FromBody] AddSimilarQuestionModel model)
//    {
//        var result = await questionService.AddSimilarQuestion(model);
//        return Ok(result);
//    }

//    [HttpPost("PassiveSimilarQuestion")]
//    public async Task<IActionResult> PassiveSimilarQuestion([FromBody] Guid similarQuestionId)
//    {
//        var result = await questionService.PassiveSimilarQuestion(similarQuestionId);
//        return Ok(result);
//    }

//    [HttpPost("ActiveSimilarQuestion")]
//    public async Task<IActionResult> ActiveSimilarQuestion([FromBody] Guid similarQuestionId)
//    {
//        var result = await questionService.ActiveSimilarQuestion(similarQuestionId);
//        return Ok(result);
//    }

//    #endregion SimilarQuestion
//}