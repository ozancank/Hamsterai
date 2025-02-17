using Application.Features.Notifications.Dto;
using Application.Services.NotificationService;
using DataAccess.EF;
using OCK.Core.Constants;
using OCK.Core.Logging.Serilog;

namespace Application.Services.BookService;

public sealed class BookManager(IDbContextFactory<HamsteraiDbContext> contextFactory,
                                INotificationService notificationService,
                                LoggerServiceBase logger) : IBookService
{

    public async Task BookPrepare(int bookId, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested) return;
        using var context = contextFactory.CreateDbContext();

        var book = await context.Books.AsNoTracking().FirstOrDefaultAsync(x => x.Id == bookId, cancellationToken: cancellationToken);
        if (book == null) return;
        if (book.Status == BookStatus.Ready) return;
        if (book.TryPrepareCount >= 3) return;
        Console.WriteLine($"Pdf {bookId} start: {book.TryPrepareCount}");

        var folderPath = Path.Combine(AppOptions.BookFolderPath, $"{bookId}");
        var originalPath = Path.Combine(folderPath, Strings.OriginalPdf);
        var thumbPath = Path.Combine(folderPath, Strings.ThumbJpg);

        try
        {
            _ = await context.Books
                .Where(x => x.Id == bookId)
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.Status, BookStatus.Prepared), cancellationToken);

            await PdfTools.SplitPdf(originalPath, folderPath);
            Console.WriteLine($"Pdf {bookId} is splitted.");

            var base64Thumb = await PdfTools.PdfToImageBase64ByImageSharp(Path.Combine(folderPath, "1.pdf"), 0, cancellationToken: cancellationToken);
            await ImageTools.Base64ToImageFile(base64Thumb, thumbPath, cancellationToken: cancellationToken);
            Console.WriteLine($"ThumbJpg {bookId} is created.");
            await Task.Delay(500, cancellationToken);

            for (var i = 1; i <= book.PageCount; i++)
            {
                if (File.Exists(Path.Combine(folderPath, $"{i}.webp")) && File.Exists(Path.Combine(folderPath, $"{i}_.webp"))) continue;
                using var stream = new FileStream(Path.Combine(folderPath, $"{i}.pdf"), FileMode.OpenOrCreate);
                if (stream.Length == 0) continue;
                var base64 = await PdfTools.PdfToImageBase64ByImageMagick(stream, 0, ImageTools.CreateFormatImageMagick(".webp"), cancellationToken: cancellationToken);
                await ImageTools.Base64ToImageFileWithResizeByImageMagick(base64, Path.Combine(folderPath, $"{i}_.webp"), 288, ImageResizeType.Height, 72, ImageTools.CreateFormatImageMagick(".webp"), cancellationToken);
                await ImageTools.Base64ToImageFileWithResizeByImageMagick(base64, Path.Combine(folderPath, $"{i}.webp"), 1298, ImageResizeType.Height, 72, ImageTools.CreateFormatImageMagick(".webp"), cancellationToken);
                Console.WriteLine($"Pdf {bookId}: Page {i} is converted to image.");
            }
            Console.WriteLine($"Pdf {bookId} is created to images.");

            _ = await context.Books
                .Where(x => x.Id == bookId)
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.IsActive, true)
                                          .SetProperty(p => p.ThumbBase64, base64Thumb)
                                          .SetProperty(p => p.Status, BookStatus.Ready), cancellationToken);

            if (File.Exists(originalPath))
            {
                File.Delete(originalPath);
                Console.WriteLine($"Pdf {bookId} original is deleted.");
            }

            var datas = new Dictionary<string, string> {
                { "id", book.Id.ToString() },
                { "type", NotificationTypes.BookReady.ToString()},
            };

            var notification = new NotificationUserDto(
                Strings.BookReadyTitle, Strings.BookReadyMessage,
                NotificationTypes.BookReady, [book.CreateUser], datas, book.Id.ToString(), 1);

            await notificationService.PushNotificationByUserId(notification);
        }
        catch (Exception ex)
        {
            ++book.TryPrepareCount;
            _ = await context.Books
                .Where(x => x.Id == bookId)
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.TryPrepareCount, book.TryPrepareCount), cancellationToken);
            context.Dispose();
            var message = $"BookPrepare - {book.Id}: {ex.Message}";
            logger.Error(message);
            Console.WriteLine(message);
        }
        finally
        {
            //_ = BookPrepare(bookId, cancellationToken);
        }
    }
}