using Application.Services.CommonService;

namespace Application.Features.Books.Rules;

public class BookRules(IBookDal bookDal,
                       ICommonService commonService) : IBusinessRule
{
    internal static Task BookShouldExists(object book)
    {
        if (book == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Book);
        return Task.CompletedTask;
    }

    internal static async Task BookShouldExistsAndActive(Book book)
    {
        await BookShouldExists(book);
        if (!book.IsActive) throw new BusinessException(Strings.DynamicNotFoundOrActive, Strings.Book);
    }

    internal async Task BookShouldExistsAndActive(int id)
    {
        var book = await bookDal.GetAsync(x => x.Id == id, enableTracking: false);
        await BookShouldExistsAndActive(book);
    }

    internal async Task BookShouldExistsAndActiveById(int id, int schoolId)
    {
        var book = await bookDal.GetAsync(x => x.Id == id && x.SchoolId == schoolId, enableTracking: false);
        await BookShouldExistsAndActive(book);
    }

    internal async Task CanAccessBook(int id)
    {
        var userType = commonService.HttpUserType;
        var schoolId = commonService.HttpSchoolId;

        var book = await bookDal.GetAsync(
            enableTracking: false,
            predicate: userType switch
            {
                UserTypes.Administator => x => x.Id == id,
                UserTypes.School or UserTypes.Teacher => x => x.IsActive && x.Id == id && x.SchoolId == schoolId,
                UserTypes.Student => x => x.IsActive && x.Id == id && x.SchoolId == schoolId && x.BookClassRooms.Any(x => x.ClassRoom != null && x.ClassRoom.Students.Any(x => x.Id == commonService.HttpConnectionId && x.IsActive)),
                _ => x => false
            },
            include: x => x.Include(x => x.BookClassRooms).ThenInclude(x => x.ClassRoom).ThenInclude(x => x!.Students));

        await BookShouldExistsAndActive(book);
    }
}