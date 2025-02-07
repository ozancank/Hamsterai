using Application.Features.Books.Dto;
using Application.Features.Books.Models.BookQuizzes;
using Application.Features.Books.Rules;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using System.Text.Json;

namespace Application.Features.Books.Queries.BookQuizzes;

public sealed class GetBookQuizByIdQuery : IRequest<GetBookQuizModel?>, ISecuredRequest<UserTypes>
{
    public int BookId { get; set; }
    public Guid BookQuizId { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [UserTypes.School, UserTypes.Teacher, UserTypes.Student];
    public bool AllowByPass => false;
}

public sealed class GetBookQuizByIdQueryHandler(IMapper mapper,
                                                IBookQuizDal bookQuizDal,
                                                BookRules bookRules) : IRequestHandler<GetBookQuizByIdQuery, GetBookQuizModel?>
{
    public async Task<GetBookQuizModel?> Handle(GetBookQuizByIdQuery request, CancellationToken cancellationToken)
    {
        await bookRules.CanAccessBook(request.BookId);

        var bookQuiz = await bookQuizDal.GetAsync(
            enableTracking: request.Tracking,
            predicate: x => x.BookId == request.BookId && x.Id == request.BookQuizId,
            include: x => x.Include(x => x.Book).Include(x => x.Lesson),
            cancellationToken: cancellationToken);

        var result = mapper.Map<GetBookQuizModel>(bookQuiz);

        if (result != null)
            result.RightAnswers = JsonSerializer.Deserialize<OptionDto[]>(bookQuiz.Answers ?? "[]") ?? [];

        if (request.ThrowException) await BookQuizRules.BookQuizShouldExists(bookQuiz);

        return result;
    }
}