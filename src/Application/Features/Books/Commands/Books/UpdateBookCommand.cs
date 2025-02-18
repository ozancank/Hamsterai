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

public class UpdateBookCommand : IRequest<GetBookModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required UpdateBookModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.School];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [$"{nameof(Model)}.{nameof(Model.File)}"];
}

public class UpdateBookCommandHandler(IMapper mapper,
                                      ICommonService commonService,
                                      IBookDal bookDal,
                                      IRBookClassRoomDal bookClassRoomDal,
                                      IBookService bookService,
                                      INotificationService notificationService,
                                      LessonRules lessonRules,
                                      ClassRoomRules classRoomRules,
                                      SchoolRules schoolRules,
                                      PublisherRules publisherRules) : IRequestHandler<UpdateBookCommand, GetBookModel>
{
    public async Task<GetBookModel> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        if (commonService.HttpUserType == UserTypes.School) request.Model.SchoolId = commonService.HttpSchoolId ?? 0;

        var book = await bookDal.GetAsync(x => x.Id == request.Model.Id && x.SchoolId == request.Model.SchoolId, cancellationToken: cancellationToken);

        await BookRules.BookShouldExistsAndActive(book);

        await schoolRules.SchoolShouldExistsAndActive(request.Model.SchoolId);
        await lessonRules.LessonShouldExistsAndActive(request.Model.LessonId);
        await publisherRules.PublisherShouldExistsAndActive(request.Model.PublisherId);

        var date = DateTime.Now;

        mapper.Map(request.Model, book);

        var pageCount = book.PageCount;
        if (request.Model.File != null)
        {
            var folderPath = Path.Combine(AppOptions.BookFolderPath, $"{request.Model.Id}");
            if (Directory.Exists(folderPath)) Directory.Delete(folderPath, true);
            Directory.CreateDirectory(folderPath);
            var originalPath = Path.Combine(folderPath, Strings.OriginalPdf);

            using (var stream = new FileStream(originalPath, FileMode.Create))
            {
                await request.Model.File!.CopyToAsync(stream, cancellationToken);
            }

            book.IsActive = false;
            book.Status = BookStatus.Updated;
        }

        book.UpdateDate = date;
        book.UpdateUser = commonService.HttpUserId;
        book.PageCount = pageCount;
        book.ThumbBase64 = string.Empty;
        book.TryPrepareCount = 0;

        var deleteList = await bookClassRoomDal.GetListAsync(predicate: x => x.BookId == book.Id, cancellationToken: cancellationToken);

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
                    BookId = book.Id,
                    ClassRoomId = classRoomId,
                });
            }
        }

        await bookDal.ExecuteWithTransactionAsync(async () =>
        {
            await bookDal.UpdateAsync(book, cancellationToken: cancellationToken);
            await bookClassRoomDal.DeleteRangeAsync(deleteList, cancellationToken: cancellationToken);
            await bookClassRoomDal.AddRangeAsync(bookClassRooms, cancellationToken: cancellationToken);
        }, cancellationToken: cancellationToken);

        var result = await bookDal.GetAsyncAutoMapper<GetBookModel>(
            enableTracking: false,
            predicate: x => x.Id == book.Id,
            include: x => x.Include(u => u.BookClassRooms).ThenInclude(x => x.ClassRoom)
                           .Include(u => u.Publisher)
                           .Include(u => u.School),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        var datas = new Dictionary<string, string> {
                { "id", result.Id.ToString() },
                { "type", NotificationTypes.BookPreparing.ToString()},
            };

        if (request.Model.File != null)
        {
            _ = bookService.BookPrepare(book.Id, cancellationToken);

            var notification = new NotificationUserDto(
                Strings.BookPreparingTitle, Strings.BookPreparingMessage,
                NotificationTypes.BookReady, [result.CreateUser], datas, result.Id.ToString(), 1);

            _ = notificationService.PushNotificationByUserId(notification);
        }

        return result;
    }
}

public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotNull().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.Id).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Book]);

        RuleFor(x => x.Model.SchoolId).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.School]);

        RuleFor(x => x.Model.LessonId).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Lesson]);

        RuleFor(x => x.Model.PublisherId).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Publisher]);

        RuleFor(x => x.Model.Name.EmptyOrTrim()).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Model.Name.EmptyOrTrim()).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Model.Name.EmptyOrTrim()).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "100"]);

        RuleFor(x => x.Model.Year).InclusiveBetween((short)1900, (short)3000).WithMessage(Strings.DynamicBetween, [Strings.Year, "1900", "3000"]);

        RuleFor(x => Path.GetExtension(x.Model.File != null ? x.Model.File.FileName.ToLowerInvariant() : string.Empty)).Equal(".pdf").When(x => x.Model.File != null).WithMessage(Strings.DynamicExtension, [Strings.File, ".pdf"]);
        RuleFor(x => x.Model.File != null ? x.Model.File.ContentType : string.Empty).Equal("application/pdf").When(x => x.Model.File != null).WithMessage(Strings.DynamicFileType, [Strings.File, "pdf"]);
        RuleFor(x => x.Model.File != null ? x.Model.File.Length : 0).LessThanOrEqualTo(629_145_600).WithMessage(Strings.DynamicMaximumFileSize, [Strings.File, "600 MB"]);
    }
}