namespace Application.Features.Books.Rules;

public class BookRules(IBookDal bookDal) : IBusinessRule
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

    internal async Task BookShouldExistsAndActiveById(int id)
    {
        var book = await bookDal.GetAsync(x => x.Id == id, enableTracking: false);
        await BookShouldExistsAndActive(book);
    }

    internal async Task BookShouldExistsAndActiveById(int id, int schoolId)
    {
        var book = await bookDal.GetAsync(x => x.Id == id && x.SchoolId == schoolId, enableTracking: false);
        await BookShouldExistsAndActive(book);
    }
}