using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Books.Queries.Books;

public sealed class GetBookThumbSmallQuery : IRequest<MemoryStream?>, ISecuredRequest<UserTypes>
{
    public int BookId { get; set; }
    public short Page { get; set; }
    public string Extension { get; set; } = ".jpg";

    public UserTypes[] Roles { get; } = [UserTypes.School, UserTypes.Teacher, UserTypes.Student];
    public bool AllowByPass => false;
}

public sealed class GetBookThumbSmallQueryHandler(IBookDal bookDal,
                                                  ICommonService commonService) : IRequestHandler<GetBookThumbSmallQuery, MemoryStream?>
{
    public async Task<MemoryStream?> Handle(GetBookThumbSmallQuery request, CancellationToken cancellationToken)
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
        if (book == null) return null;

        var filePath = Path.Combine(AppOptions.BookFolderPath, $"{request.BookId}", $"{request.Page}_{request.Extension}");
        if (!File.Exists(filePath)) return null;

        var memory = new MemoryStream();
        await using (var stream = new FileStream(filePath, FileMode.Open))
        {
            await stream.CopyToAsync(memory, cancellationToken);
        }
        memory.Position = 0;

        return memory;
    }
}