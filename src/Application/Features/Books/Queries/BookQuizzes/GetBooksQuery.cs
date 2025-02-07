using Application.Features.Books.Dto;
using Application.Features.Books.Models.BookQuizzes;
using Application.Features.Books.Rules;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using System.Text.Json;

namespace Application.Features.Books.Queries.BookQuizzes;

public class GetBookQuizzesQuery : IRequest<PageableModel<GetBookQuizModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required BookQuizRequestModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School, UserTypes.Teacher, UserTypes.Student];
    public bool AllowByPass => false;
}

public class GetBookQuizzesQueryHandler(IMapper mapper,
                                        IBookQuizDal bookQuizDal,
                                        BookRules bookRules) : IRequestHandler<GetBookQuizzesQuery, PageableModel<GetBookQuizModel>>
{
    public async Task<PageableModel<GetBookQuizModel>> Handle(GetBookQuizzesQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.Model ??= new BookQuizRequestModel();

        await bookRules.CanAccessBook(request.Model.BookId);

        var bookQuizzes = await bookQuizDal.GetPageListAsync(
        enableTracking: false,
        predicate: x => x.IsActive
                        && x.BookId == request.Model.BookId
                        && (request.Model.LessonId <= 0 || x.LessonId == request.Model.LessonId),
        include: x => x.Include(u => u.Book).Include(x => x.Lesson),
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