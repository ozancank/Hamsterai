using Application.Features.Books.Commands.Books;
using Application.Features.Books.Commands.Publishers;
using Application.Features.Books.Models.Books;
using Application.Features.Books.Models.Publisher;
using Application.Features.Books.Queries.Books;
using Application.Features.Books.Queries.Publishers;
using Asp.Versioning;

namespace WebAPI.Controllers.V1;

[ApiController]
[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
[ApiVersion("1")]
public class BookController : BaseController
{
    #region Book

    [HttpGet("GetBookPage/{bookId}/{page}")]
    public async Task<IActionResult> GetBookPage([FromRoute] int bookId, [FromRoute] short page)
    {
        var query = new GetBookPageQuery { BookId = bookId, Page = page };
        var result = await Mediator.Send(query);
        return result != null ? File(result, "application/pdf") : NotFound();
    }

    [HttpGet("GetBookPageImage/{bookId}/{page}")]
    public async Task<IActionResult> GetBookPageImage([FromRoute] int bookId, [FromRoute] short page)
    {
        var query = new GetBookPageImageQuery { BookId = bookId, Page = page, Extension = ".webp" };
        var result = await Mediator.Send(query);
        return result != null ? File(result, "image/webp") : NotFound();
    }

    [HttpGet("GetBookThumb/{bookId}")]
    public async Task<IActionResult> GetBookThumb([FromRoute] int bookId)
    {
        var query = new GetBookThumbQuery { BookId = bookId };
        var result = await Mediator.Send(query);
        return result != null ? File(result, "image/jpeg") : NotFound();
    }

    [HttpGet("GetBookThumbSmall/{bookId}/{page}")]
    public async Task<IActionResult> GetBookThumbSmall([FromRoute] int bookId, [FromRoute] short page)
    {
        var query = new GetBookThumbSmallQuery { BookId = bookId, Page = page, Extension=".webp" };
        var result = await Mediator.Send(query);
        return result != null ? File(result, "image/webp") : NotFound();
    }

    [HttpGet("GetBookById/{id}")]
    public async Task<IActionResult> GetBookById([FromRoute] int id)
    {
        var query = new GetBookByIdQuery { Id = id };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("GetBooks")]
    public async Task<IActionResult> GetBooks([FromQuery] PageRequest pageRequest, [FromBody] BookRequestModel model)
    {
        var query = new GetBooksQuery { PageRequest = pageRequest, Model = model };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("GetBooksDynamic")]
    public async Task<IActionResult> GetBooksDynamic([FromQuery] PageRequest pageRequest, [FromBody] Dynamic dynamicModel)
    {
        var query = new GetBooksByDynamicQuery { PageRequest = pageRequest, Dynamic = dynamicModel };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("AddBook")]
    public async Task<IActionResult> AddBook([FromForm] AddBookModel model)
    {
        var command = new AddBookCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("UpdateBook")]
    public async Task<IActionResult> UpdateBook([FromForm] UpdateBookModel model)
    {
        var command = new UpdateBookCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("UpdateBookClassRoom")]
    public async Task<IActionResult> UpdateBookClassRoom([FromForm] UpdateBookClassRoomModel model)
    {
        var command = new UpdateBookClassRoomCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("PassiveBook")]
    public async Task<IActionResult> PassiveBook([FromQuery] int id)
    {
        var command = new PassiveBookCommand { BookId = id };
        await Mediator.Send(command);
        return Ok();
    }

    [HttpPost("ActiveBook")]
    public async Task<IActionResult> ActiveBook([FromQuery] int id)
    {
        var command = new ActiveBookCommand { BookId = id };
        await Mediator.Send(command);
        return Ok();
    }

    #endregion Book

    #region Publisher

    [HttpGet("GetPublisherById/{id}")]
    public async Task<IActionResult> GetPublisherById([FromRoute] int id)
    {
        var query = new GetPublisherByIdQuery { Id = id };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("GetPublishers")]
    public async Task<IActionResult> GetPublishers([FromQuery] PageRequest pageRequest)
    {
        var query = new GetPublishersQuery { PageRequest = pageRequest };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("GetPublishersDynamic")]
    public async Task<IActionResult> GetPublishersDynamic([FromQuery] PageRequest pageRequest, [FromBody] Dynamic dynamicModel)
    {
        var query = new GetPublishersByDynamicQuery { PageRequest = pageRequest, Dynamic = dynamicModel };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("AddPublisher")]
    public async Task<IActionResult> AddPublisher([FromBody] AddPublisherModel model)
    {
        var command = new AddPublisherCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("UpdatePublisher")]
    public async Task<IActionResult> UpdatePublisher([FromBody] UpdatePublisherModel model)
    {
        var command = new UpdatePublisherCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("PassivePublisher")]
    public async Task<IActionResult> PassivePublisher([FromQuery] short id)
    {
        var command = new PassivePublisherCommand { PublisherId = id };
        await Mediator.Send(command);
        return Ok();
    }

    [HttpPost("ActivePublisher")]
    public async Task<IActionResult> ActivePublisher([FromQuery] short id)
    {
        var command = new ActivePublisherCommand { PublisherId = id };
        await Mediator.Send(command);
        return Ok();
    }

    #endregion Publisher
}