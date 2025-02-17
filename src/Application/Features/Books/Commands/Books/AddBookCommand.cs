using Application.Features.Books.Models.Books;
using Application.Features.Books.Rules;
using Application.Features.Lessons.Rules;
using Application.Features.Notifications.Dto;
using Application.Features.Schools.Rules;
using Application.Services.BookService;
using Application.Services.CommonService;
using Application.Services.NotificationService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Books.Commands.Books;

public class AddBookCommand : IRequest<GetBookModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required AddBookModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.School];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [$"{nameof(Model)}.{nameof(Model.File)}"];
}

public class AddBookCommandHandler(IMapper mapper,
                                   ICommonService commonService,
                                   IBookDal bookDal,
                                   IRBookClassRoomDal bookClassRoomDal,
                                   IBookService bookService,
                                   INotificationService notificationService,
                                   LessonRules lessonRules,
                                   ClassRoomRules classRoomRules,
                                   SchoolRules schoolRules,
                                   PublisherRules publisherRules) : IRequestHandler<AddBookCommand, GetBookModel>
{
    public async Task<GetBookModel> Handle(AddBookCommand request, CancellationToken cancellationToken)
    {
        if (commonService.HttpUserType == UserTypes.School) request.Model.SchoolId = commonService.HttpSchoolId ?? 0;

        await schoolRules.SchoolShouldExistsAndActive(request.Model.SchoolId);
        await lessonRules.LessonShouldExistsAndActive(request.Model.LessonId);
        await publisherRules.PublisherShouldExistsAndActive(request.Model.PublisherId);

        var date = DateTime.Now;
        var bookId = await bookDal.GetNextPrimaryKeyAsync(x => x.Id, cancellationToken: cancellationToken);
        var folderPath = Path.Combine(AppOptions.BookFolderPath, $"{bookId}");
        if (Directory.Exists(folderPath)) Directory.Delete(folderPath, true);
        Directory.CreateDirectory(folderPath);
        var originalPath = Path.Combine(folderPath, Strings.OriginalPdf);

        using (var stream = new FileStream(originalPath, FileMode.Create))
        {
            await request.Model.File!.CopyToAsync(stream, cancellationToken);
        }

        var book = mapper.Map<Book>(request.Model);
        book.Id = bookId;
        book.IsActive = false;
        book.CreateDate = date;
        book.CreateUser = commonService.HttpUserId;
        book.UpdateDate = date;
        book.UpdateUser = commonService.HttpUserId;
        book.PageCount = PdfTools.PdfPageCount(originalPath).ToShort();
        book.ThumbBase64 = string.Empty;
        book.TryPrepareCount = 0;
        book.Status = BookStatus.Added;

        List<RBookClassRoom> bookClassRooms = [];

        if (request.Model.ClassRoomIds != null)
        {
            foreach (var classRoomId in request.Model.ClassRoomIds)
            {
                await classRoomRules.ClassRoomShouldExistsAndActiveById(classRoomId);
                bookClassRooms.Add(new()
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    CreateDate = date,
                    CreateUser = commonService.HttpUserId,
                    UpdateDate = date,
                    UpdateUser = commonService.HttpUserId,
                    BookId = bookId,
                    ClassRoomId = classRoomId,
                });
            }
        }

        await bookDal.ExecuteWithTransactionAsync(async () =>
        {
            await bookDal.AddAsync(book, cancellationToken: cancellationToken);
            if (bookClassRooms.Count != 0)
                await bookClassRoomDal.AddRangeAsync(bookClassRooms, cancellationToken: cancellationToken);
        }, cancellationToken: cancellationToken);

        var datas = new Dictionary<string, string> {
                { "id", book.Id.ToString() },
                { "type", NotificationTypes.BookPreparing.ToString()},
            };

        var notification = new NotificationUserDto(
            Strings.BookPreparingTitle, Strings.BookPreparingMessage,
            NotificationTypes.BookReady, [book.CreateUser], datas, book.Id.ToString(), 1);

        await notificationService.PushNotificationByUserId(notification);

        _ = bookService.BookPrepare(book.Id, cancellationToken);

        var result = await bookDal.GetAsyncAutoMapper<GetBookModel>(
            enableTracking: false,
            predicate: x => x.Id == bookId,
            include: x => x.Include(u => u.BookClassRooms).ThenInclude(x => x.ClassRoom)
                           .Include(u => u.Publisher)
                           .Include(u => u.School),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        return result;
    }
}

public class AddBookCommandValidator : AbstractValidator<AddBookCommand>
{
    public AddBookCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotNull().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.SchoolId).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.School]);

        RuleFor(x => x.Model.LessonId).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Lesson]);

        RuleFor(x => x.Model.PublisherId).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Publisher]);

        RuleFor(x => x.Model.Name.EmptyOrTrim()).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Model.Name.EmptyOrTrim()).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Model.Name.EmptyOrTrim()).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "100"]);

        RuleFor(x => x.Model.Year).InclusiveBetween((short)1900, (short)3000).WithMessage(Strings.DynamicBetween, [Strings.Year, "1900", "3000"]);

        RuleFor(x => x.Model.File).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.File]);
        RuleFor(x => Path.GetExtension(x.Model.File != null ? x.Model.File.FileName.ToLowerInvariant() : string.Empty)).Equal(".pdf").WithMessage(Strings.DynamicExtension, [Strings.File, ".pdf"]);
        RuleFor(x => x.Model.File != null ? x.Model.File.ContentType : string.Empty).Equal("application/pdf").WithMessage(Strings.DynamicFileType, [Strings.File, "pdf"]);
        RuleFor(x => x.Model.File != null ? x.Model.File.Length : 0).LessThanOrEqualTo(629_145_600).WithMessage(Strings.DynamicMaximumFileSize, [Strings.File, "600 MB"]);
    }
}