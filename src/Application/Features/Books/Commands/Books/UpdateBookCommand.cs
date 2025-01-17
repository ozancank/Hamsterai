using Application.Features.Books.Models.Books;
using Application.Features.Books.Rules;
using Application.Features.Lessons.Rules;
using Application.Features.Schools.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Books.Commands.Books;

public class UpdateBookCommand : IRequest<GetBookModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required UpdateBookModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.School];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = ["UpdateBookModel.File"];
}

public class UpdateBookCommandHandler(IMapper mapper,
                                      ICommonService commonService,
                                      IBookDal bookDal,
                                      IRBookClassRoomDal bookClassRoomDal,
                                      LessonRules lessonRules,
                                      ClassRoomRules classRoomRules,
                                      SchoolRules schoolRules,
                                      PublisherRules publisherRules) : IRequestHandler<UpdateBookCommand, GetBookModel>
{
    public async Task<GetBookModel> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        if (commonService.HttpUserType == UserTypes.School) request.Model.SchoolId = commonService.HttpSchoolId ?? 0;

        var book = await bookDal.GetAsync(x => x.Id == request.Model.Id && x.SchoolId == request.Model.SchoolId, cancellationToken: cancellationToken);

        BookRules.BookShouldExistsAndActive(book);

        await schoolRules.SchoolShouldExistsAndActive(request.Model.SchoolId);
        await lessonRules.LessonShouldExistsAndActive(request.Model.LessonId);
        await publisherRules.PublisherShouldExistsAndActive(request.Model.PublisherId);

        var date = DateTime.Now;

        var pageCount = book.PageCount;
        if (request.Model.File != null)
        {
            var folderPath = Path.Combine(AppOptions.BookFolderPath, $"{request.Model.Id}");
            if (Directory.Exists(folderPath)) Directory.Delete(folderPath, true);
            Directory.CreateDirectory(folderPath);
            var thumbPath = Path.Combine(folderPath, Strings.ThumbnailName);

            using (var stream = request.Model.File!.OpenReadStream())
            {
                pageCount = (short)PdfTools.PdfPageCount(stream);
                PdfTools.SplitPdf(stream, folderPath);
                var base64 = await PdfTools.PdfToImageBase64(stream, 0);
                await ImageTools.Base64ToImageFile(base64, thumbPath, cancellationToken: cancellationToken);
            }
        }

        mapper.Map(request.Model, book);
        book.UpdateDate = date;
        book.UpdateUser = commonService.HttpUserId;
        book.PageCount = pageCount;
        book.Year = book.Year == 0 ? null : book.Year;

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

        RuleFor(x => x.Model.Year).InclusiveBetween((short)1900, (short)3000).When(x => x.Model.Year != 0).WithMessage(Strings.DynamicBetween, [Strings.Year, "1900", "3000"]);

        RuleFor(x => Path.GetExtension(x.Model.File != null ? x.Model.File.FileName.ToLowerInvariant() : string.Empty)).Equal(".pdf").When(x => x.Model.File != null).WithMessage(Strings.DynamicExtension, [Strings.File, ".pdf"]);
        RuleFor(x => x.Model.File != null ? x.Model.File.ContentType : string.Empty).Equal("application/pdf").When(x => x.Model.File != null).WithMessage(Strings.DynamicFileType, [Strings.File, "pdf"]);
        RuleFor(x => x.Model.File != null ? x.Model.File.Length : 0).LessThanOrEqualTo(268_435_456).When(x => x.Model.File != null).WithMessage(Strings.DynamicMaximumFileSize, [Strings.File, "256 MB"]);
    }
}