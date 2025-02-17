using Application.Features.Books.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Books.Commands.Books;

public class ActiveBookCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required int BookId { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class ActiveBookCommandHandler(IBookDal bookDal,
                                      IRBookClassRoomDal bookClassRoomDal,
                                      ICommonService commonService) : IRequestHandler<ActiveBookCommand, bool>
{
    public async Task<bool> Handle(ActiveBookCommand request, CancellationToken cancellationToken)
    {
        var userId = commonService.HttpUserId;
        var userType = commonService.HttpUserType;
        var schoolId = commonService.HttpSchoolId;
        var date = DateTime.Now;

        var book = await bookDal.GetAsync(predicate: x => x.Id == request.BookId && (userType == UserTypes.Administator || x.SchoolId == schoolId), cancellationToken: cancellationToken);
        await BookRules.BookShouldExists(book);
        await BookRules.BookShouldBeReady(book);

        var bookClassRooms = await bookClassRoomDal.GetListAsync(predicate: x => x.BookId == request.BookId, cancellationToken: cancellationToken);

        book.UpdateUser = userId;
        book.UpdateDate = date;
        book.IsActive = true;

        foreach (var item in bookClassRooms)
        {
            item.UpdateUser = userId;
            item.UpdateDate = date;
            item.IsActive = true;
        }

        await bookDal.ExecuteWithTransactionAsync(async () =>
        {
            if (bookClassRooms.Count > 0) await bookClassRoomDal.UpdateRangeAsync(bookClassRooms, cancellationToken: cancellationToken);
            await bookDal.UpdateAsync(book, cancellationToken: cancellationToken);
        }, cancellationToken: cancellationToken);

        return true;
    }
}