using Application.Features.Books.Commands.BookQuizzes;
using Application.Features.Books.Commands.Books;
using Application.Features.Books.Commands.Publishers;
using Application.Features.Books.Models.BookQuizzes;
using Application.Features.Books.Models.Books;
using Application.Features.Books.Models.Publisher;
using Application.Features.Books.Queries.BookQuizzes;
using Application.Features.Books.Queries.Books;
using Application.Features.Books.Queries.Publishers;
using Asp.Versioning;
using Domain.Constants;
using System.Text;

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

    [HttpGet("GetBookPageImageAll/{bookId}")]
    public async Task GetBookPageImageAll([FromRoute] int bookId, CancellationToken cancellationToken = default)
    {
        var query = new GetBookByIdQuery { Id = bookId };
        var book = await Mediator.Send(query, cancellationToken);
        var folderPath = Path.Combine(AppOptions.BookFolderPath, $"{bookId}");
        if (book == null || !Directory.Exists(folderPath))
        {
            Response.StatusCode = 404;
            return;
        }

        Response.ContentType = "application/octet-stream";
        var streamWriter = new StreamWriter(Response.Body, Encoding.UTF8);

        for (int i = 1; i <= book.PageCount; i++)
        {
            var fileName = $"{i}.webp";
            var filePath = Path.Combine(folderPath, fileName);
            if (!System.IO.File.Exists(filePath)) continue;

            var imageBytes = await System.IO.File.ReadAllBytesAsync(filePath, cancellationToken);
            var response = $"{i}^#^{Convert.ToBase64String(imageBytes)}\n";

            await streamWriter.WriteAsync(response);
            await streamWriter.FlushAsync(cancellationToken);
        }
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
        var query = new GetBookThumbSmallQuery { BookId = bookId, Page = page, Extension = ".webp" };
        var result = await Mediator.Send(query);
        return result != null ? File(result, "image/webp") : NotFound();
    }

    [HttpGet("GetBookThumbSmallAll/{bookId}")]
    public async Task GetBookThumbSmallAll([FromRoute] int bookId, CancellationToken cancellationToken = default)
    {
        var query = new GetBookByIdQuery { Id = bookId };
        var book = await Mediator.Send(query, cancellationToken);
        var folderPath = Path.Combine(AppOptions.BookFolderPath, $"{bookId}");
        if (book == null || !Directory.Exists(folderPath))
        {
            Response.StatusCode = 404;
            return;
        }

        Response.ContentType = "application/octet-stream";
        var streamWriter = new StreamWriter(Response.Body, Encoding.UTF8);

        for (int i = 1; i <= book.PageCount; i++)
        {
            var fileName = $"{i}_.webp";
            var filePath = Path.Combine(folderPath, fileName);
            if (!System.IO.File.Exists(filePath)) continue;

            var imageBytes = await System.IO.File.ReadAllBytesAsync(filePath, cancellationToken);
            var response = $"{i}^#^{Convert.ToBase64String(imageBytes)}\n";

            await streamWriter.WriteAsync(response);
            await streamWriter.FlushAsync(cancellationToken);
        }
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

    #region BookQuiz

    [HttpGet("GetBookQuizById/{bookId}/{id}")]
    public async Task<IActionResult> GetBookQuizById([FromRoute] int bookId, [FromRoute] Guid id)
    {
        var query = new GetBookQuizByIdQuery { BookId = bookId, BookQuizId = id };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("GetBookQuizzes")]
    public async Task<IActionResult> GetBookQuizzes([FromQuery] PageRequest pageRequest, [FromBody] BookQuizRequestModel model)
    {
        var query = new GetBookQuizzesQuery { PageRequest = pageRequest, Model = model };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("GetBookQuizzesDynamic")]
    public async Task<IActionResult> GetBookQuizzesDynamic([FromQuery] PageRequest pageRequest, [FromQuery] int bookId, [FromBody] Dynamic dynamic)
    {
        var query = new GetBookQuizzesByDynamicQuery { PageRequest = pageRequest, BookId = bookId, Dynamic = dynamic };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("AddBookQuiz")]
    public async Task<IActionResult> AddBookQuiz([FromBody] AddBookQuizModel model)
    {
        var command = new AddBookQuizCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("UpdateBookQuiz")]
    public async Task<IActionResult> UpdateBookQuiz([FromBody] UpdateBookQuizModel model)
    {
        var command = new UpdateBookQuizCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("PassiveBookQuiz")]
    public async Task<IActionResult> PassiveBookQuiz([FromQuery] Guid id)
    {
        var command = new PassiveBookQuizCommand { BookQuizId = id };
        await Mediator.Send(command);
        return Ok();
    }

    [HttpPost("ActiveBookQuiz")]
    public async Task<IActionResult> ActiveBookQuiz([FromQuery] Guid id)
    {
        var command = new ActiveBookQuizCommand { BookQuizId = id };
        await Mediator.Send(command);
        return Ok();
    }

    [HttpPost("SendBookQuiz")]
    public async Task<IActionResult> SendBookQuiz([FromBody] UpdateBookQuizUserModel model)
    {
        var command = new UpdateBookQuizUserCommand { Model = model };
        await Mediator.Send(command);
        return Ok();
    }

    [HttpGet("GetBookQuizUserById")]
    public async Task<IActionResult> GetBookQuizUserById([FromQuery] int bookId, [FromQuery] Guid bookQuizId, [FromQuery] long userId)
    {
        var query = new GetBookQuizUserByIdQuery { BookId = bookId, BookQuizId = bookQuizId, UserId = userId };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    #endregion BookQuiz
}