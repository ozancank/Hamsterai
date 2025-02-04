using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Books.Queries.Books;

public sealed class GetBookThumbSmallAllQuery : IRequest<List<byte[]>>, ISecuredRequest<UserTypes>
{
    public int BookId { get; set; }
    public short PageCount { get; set; } = 0;
    public string Extension { get; set; } = ".jpg";

    public UserTypes[] Roles { get; } = [UserTypes.School, UserTypes.Teacher, UserTypes.Student];
    public bool AllowByPass => false;
}

public sealed class GetBookThumbSmallAllQueryHandler(IBookDal bookDal,
                                                     ICommonService commonService) : IRequestHandler<GetBookThumbSmallAllQuery, List<byte[]>>
{
    public async Task<List<byte[]>> Handle(GetBookThumbSmallAllQuery request, CancellationToken cancellationToken)
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
        if (book == null) return [];

        if (request.PageCount <= 0 || request.PageCount >= book.PageCount) request.PageCount = book.PageCount;

        var folderPath = Path.Combine(AppOptions.BookFolderPath, $"{request.BookId}");
        if (!Directory.Exists(folderPath)) return [];

        var result = new List<byte[]>();

        for (int i = 1; i <= request.PageCount; i++)
        {
            var fileName = $"{i}_{request.Extension}";
            var filePath = Path.Combine(folderPath, fileName);
            if (!File.Exists(filePath)) continue;

            await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            result.Add(memoryStream.ToArray());
        }

        return result;
    }
}