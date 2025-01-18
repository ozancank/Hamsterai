using Application.Features.Books.Models.Books;
using Application.Features.Books.Rules;
using Application.Features.Lessons.Rules;
using Application.Features.Schools.Rules;
using Application.Services.CommonService;
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
        var thumbPath = Path.Combine(folderPath, Strings.ThumbnailName);

        var pageCount = 0;
        using (var stream = request.Model.File!.OpenReadStream())
        {
            pageCount = PdfTools.PdfPageCount(stream);
            PdfTools.SplitPdf(stream, folderPath);
            var base64 = await PdfTools.PdfToImageBase64(stream, 0);
            await ImageTools.Base64ToImageFile(base64, thumbPath, cancellationToken: cancellationToken);
        }

        var book = mapper.Map<Book>(request.Model);
        book.Id = bookId;
        book.IsActive = true;
        book.CreateDate = date;
        book.CreateUser = commonService.HttpUserId;
        book.UpdateDate = date;
        book.UpdateUser = commonService.HttpUserId;
        book.PageCount = (short)pageCount;

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
        RuleFor(x => x.Model.File != null ? x.Model.File.Length : 0).LessThanOrEqualTo(268_435_456).WithMessage(Strings.DynamicMaximumFileSize, [Strings.File, "256 MB"]);
    }
}