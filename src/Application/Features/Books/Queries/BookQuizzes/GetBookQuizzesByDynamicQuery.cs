using Application.Features.Books.Dto;
using Application.Features.Books.Models.BookQuizzes;
using Application.Features.Books.Rules;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using System.Text.Json;

namespace Application.Features.Books.Queries.BookQuizzes;

public class GetBookQuizzesByDynamicQuery : IRequest<PageableModel<GetBookQuizModel>>, ISecuredRequest<UserTypes>
{
    public required int BookId { get; set; }
    public required PageRequest PageRequest { get; set; }
    public required Dynamic Dynamic { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School, UserTypes.Teacher, UserTypes.Student];
    public bool AllowByPass => false;
}

public class GetBookQuizzesByDynamicQueryHandler(IMapper mapper,
                                                 IBookQuizDal bookQuizDal,
                                                 BookRules bookRules) : IRequestHandler<GetBookQuizzesByDynamicQuery, PageableModel<GetBookQuizModel>>
{
    public async Task<PageableModel<GetBookQuizModel>> Handle(GetBookQuizzesByDynamicQuery request, CancellationToken cancellationToken)
    {
        await bookRules.CanAccessBook(request.BookId);

        request.PageRequest ??= new PageRequest();
        request.Dynamic ??= new Dynamic();

        var bookQuizzes = await bookQuizDal.GetPageListAsyncByDynamic(
            dynamic: request.Dynamic,
            predicate: x => x.BookId == request.BookId,
            enableTracking: false,
            include: x => x.Include(u => u.Book).Include(u => u.Lesson),
            defaultOrderColumnName: x => x.Name,
            defaultDescending: false,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetBookQuizModel>>(bookQuizzes);

        result.Items.ForEach(x =>
        {
            x.RightAnswers = JsonSerializer.Deserialize<OptionDto[]>(bookQuizzes.Items.FirstOrDefault(a => a.Id == x.Id)?.Answers ?? "[]") ?? [];
        });

        return result;
    }
}