namespace Application.Features.Books.Rules;

public class BookRules : IBusinessRule
{
    internal static Task BookShouldExists(object book)
    {
        if (book == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Book);
        return Task.CompletedTask;
    }

    internal static async void BookShouldExistsAndActive(Book book)
    {
        await BookShouldExists(book);
        if (!book.IsActive) throw new BusinessException(Strings.DynamicNotFoundOrActive, Strings.Book);
    }
}