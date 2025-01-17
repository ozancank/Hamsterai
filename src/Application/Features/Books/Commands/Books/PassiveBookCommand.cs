using Application.Features.Books.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Books.Commands.Books;

public class PassiveBookCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required int BookId { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.Teacher];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class PassiveBookCommandHandler(IBookDal bookDal,
                                       IRBookClassRoomDal bookClassRoomDal,
                                       ICommonService commonService) : IRequestHandler<PassiveBookCommand, bool>
{
    public async Task<bool> Handle(PassiveBookCommand request, CancellationToken cancellationToken)
    {
        var userId = commonService.HttpUserId;
        var userType = commonService.HttpUserType;
        var schoolId = commonService.HttpSchoolId;
        var date = DateTime.Now;

        var book = await bookDal.GetAsync(predicate: x => x.Id == request.BookId && (userType == UserTypes.Administator || x.SchoolId == schoolId), cancellationToken: cancellationToken);
        await BookRules.BookShouldExists(book);

        var bookClassRooms = await bookClassRoomDal.GetListAsync(predicate: x => x.BookId == request.BookId, cancellationToken: cancellationToken);

        book.UpdateUser = userId;
        book.UpdateDate = date;
        book.IsActive = false;

        foreach (var item in bookClassRooms)
        {
            item.UpdateUser = userId;
            item.UpdateDate = date;
            item.IsActive = false;
        }

        await bookDal.ExecuteWithTransactionAsync(async () =>
        {
            if (bookClassRooms.Count > 0) await bookClassRoomDal.UpdateRangeAsync(bookClassRooms, cancellationToken: cancellationToken);
            await bookDal.UpdateAsync(book, cancellationToken: cancellationToken);
        }, cancellationToken: cancellationToken);

        return true;
    }
}