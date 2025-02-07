using Application.Features.Notifications.Dto;
using Application.Services.NotificationService;
using DataAccess.EF;
using OCK.Core.Constants;

namespace Application.Services.BookService;

public sealed class BookManager(IDbContextFactory<HamsteraiDbContext> contextFactory,
                                INotificationService notificationService) : IBookService
{
    public async Task BookToWebp(int bookId, CancellationToken cancellationToken = default)
    {
        using var context = contextFactory.CreateDbContext();

        var book = await context.Books.AsNoTracking().FirstAsync(x => x.Id == bookId, cancellationToken: cancellationToken);

        var folderPath = Path.Combine(AppOptions.BookFolderPath, $"{bookId}");

        for (var i = 1; i <= book.PageCount; i++)
        {
            var base64 = await PdfTools.PdfToImageBase64(Path.Combine(folderPath, $"{i}.pdf"), 0, ImageTools.CreateEncoder(".webp"), cancellationToken);
            await ImageTools.Base64ToImageFileWithResize(base64, Path.Combine(folderPath, $"{i}_.webp"), 288, ImageResizeType.Height, ImageTools.CreateEncoder(".webp"), cancellationToken);
            await ImageTools.Base64ToImageFileWithResize(base64, Path.Combine(folderPath, $"{i}.webp"), 1298, ImageResizeType.Height, ImageTools.CreateEncoder(".webp"), cancellationToken);
            Console.WriteLine($"Pdf {bookId}: Page {i} is converted to image.");
        }
        Console.WriteLine($"Pdf {bookId} is created to images.");

        _ = await context.Books
            .Where(x => x.Id == bookId)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.IsActive, true), cancellationToken);

        var datas = new Dictionary<string, string> {
                { "id", book.Id.ToString() },
                { "type", NotificationTypes.BookReady.ToString()},
            };

        var notification = new NotificationUserDto(
            Strings.BookReadyTitle, Strings.BookReadyMessage,
            NotificationTypes.BookReady, [book.CreateUser], datas, book.Id.ToString(), 1);

        await notificationService.PushNotificationByUserId(notification);
    }
}