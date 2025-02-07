using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using System.Runtime.CompilerServices;

namespace Application.Features.Books.Queries.Books;

public sealed record PageResponse(short PageNumber, string ImageData);

public sealed class GetBookPageImageAllQuery : IRequest<IAsyncEnumerable<PageResponse>>, ISecuredRequest<UserTypes>
{
    public int BookId { get; set; }
    public string Extension { get; set; } = ".jpg";

    public UserTypes[] Roles { get; } = [UserTypes.School, UserTypes.Teacher, UserTypes.Student];
    public bool AllowByPass => false;
}

public sealed class GetBookPageImageAllQueryHandler(IBookDal bookDal,
                                                    ICommonService commonService) : IRequestHandler<GetBookPageImageAllQuery, IAsyncEnumerable<PageResponse>>
{
    public async Task<IAsyncEnumerable<PageResponse>> Handle(GetBookPageImageAllQuery request, CancellationToken cancellationToken)
    {
        var userType = commonService.HttpUserType;
        var schoolId = commonService.HttpSchoolId;
        var book = await bookDal.GetAsync(
            enableTracking: false,
            predicate: userType switch
            {
                UserTypes.Administator => x => x.Id == request.BookId,
                UserTypes.School or UserTypes.Teacher => x => x.IsActive && x.Id == request.BookId && x.SchoolId == schoolId,
                UserTypes.Student => x => x.IsActive && x.Id == request.BookId && x.SchoolId == schoolId && x.BookClassRooms.Any(x => x.ClassRoom != null && x.ClassRoom.Students.Any(x => x.Id == commonService.HttpConnectionId && x.IsActive)),
                _ => x => false
            },
            include: x => x.Include(x => x.BookClassRooms).ThenInclude(x => x.ClassRoom).ThenInclude(x => x!.Students),
            cancellationToken: cancellationToken);
        if (book == null) return GetEmptyAsyncEnumerable();

        var folderPath = Path.Combine(AppOptions.BookFolderPath, $"{request.BookId}");
        if (!Directory.Exists(folderPath)) return GetEmptyAsyncEnumerable();

        return GetPagesAsync(folderPath, book.PageCount, request.Extension, cancellationToken);
    }

    private static async IAsyncEnumerable<PageResponse> GetPagesAsync(string folderPath, short pageCount, string extension, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        for (short i = 1; i <= pageCount; i++)
        {
            var fileName = $"{i}{extension}";
            var filePath = Path.Combine(folderPath, fileName);
            if (!File.Exists(filePath)) continue;

            var imageBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
            yield return new PageResponse(i, Convert.ToBase64String(imageBytes));

            await Task.Delay(100, cancellationToken);
        }
    }

    private static async IAsyncEnumerable<PageResponse> GetEmptyAsyncEnumerable()
    {
        await Task.CompletedTask; 
        yield break;
    }
}