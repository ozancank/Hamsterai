using Application.Features.Books.Dto;
using Application.Features.Books.Models.BookQuizzes;
using Application.Features.Books.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using System.Text.Json;

namespace Application.Features.Books.Queries.BookQuizzes;

public sealed class GetBookQuizUserByIdQuery : IRequest<GetBookQuizUserModel>, ISecuredRequest<UserTypes>
{
    public int BookId { get; set; }
    public Guid BookQuizId { get; set; }
    public long UserId { get; set; }

    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public sealed class GetBookQuizUserByIdQueryHandler(IMapper mapper,
                                                    IBookQuizUserDal bookQuizUserDal,
                                                    ICommonService commonService,
                                                    BookRules bookRules) : IRequestHandler<GetBookQuizUserByIdQuery, GetBookQuizUserModel?>
{
    public async Task<GetBookQuizUserModel?> Handle(GetBookQuizUserByIdQuery request, CancellationToken cancellationToken)
    {
        await bookRules.CanAccessBook(request.BookId);

        if(commonService.HttpUserType != UserTypes.Administator) request.UserId = commonService.HttpUserId;

        var bookQuizUser = await bookQuizUserDal.GetAsync(
            enableTracking: request.Tracking,
            predicate: x => x.BookQuiz != null && x.BookQuiz.IsActive && x.BookQuiz.BookId == request.BookId
                            && x.BookQuiz.Id == request.BookQuizId && x.UserId == request.UserId,
            include: x => x.Include(x => x.BookQuiz).ThenInclude(x => x != null ? x.Book : default)
                           .Include(x => x.BookQuiz).ThenInclude(x => x != null ? x.Lesson : default),
            cancellationToken: cancellationToken);

        var result = mapper.Map<GetBookQuizUserModel>(bookQuizUser);

        if (result != null)
        {
            result.UserAnswers = JsonSerializer.Deserialize<OptionDto[]>(bookQuizUser.Answers ?? "[]") ?? [];
            result.RightAnswers = JsonSerializer.Deserialize<OptionDto[]>(bookQuizUser.BookQuiz?.Answers ?? "[]") ?? [];
        }

        if (request.ThrowException) await BookQuizRules.BookQuizShouldExists(bookQuizUser);

        return result;
    }
}