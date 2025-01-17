using Application.Features.Books.Models.Books;
using Application.Features.Books.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Books.Queries.Books;

public sealed class GetBookByIdQuery : IRequest<GetBookModel>, ISecuredRequest<UserTypes>
{
    public int Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [UserTypes.School, UserTypes.Teacher, UserTypes.Student];
    public bool AllowByPass => false;
}

public sealed class GetBookByIdQueryHandler(IMapper mapper,
                                            ICommonService commonService,
                                            IBookDal bookDal) : IRequestHandler<GetBookByIdQuery, GetBookModel>
{
    public async Task<GetBookModel> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var userType = commonService.HttpUserType;
        var schoolId = commonService.HttpSchoolId;

        var book = await bookDal.GetAsyncAutoMapper<GetBookModel>(
            enableTracking: false,
            predicate: userType switch
            {
                UserTypes.Administator => x => x.Id == request.Id,
                UserTypes.School or UserTypes.Teacher => x => x.IsActive && x.Id == request.Id && x.SchoolId == schoolId,
                UserTypes.Student => x => x.IsActive && x.Id == request.Id && x.SchoolId == schoolId && x.BookClassRooms.Any(x => x.ClassRoom != null && x.ClassRoom.Students.Any(x => x.Id == commonService.HttpConnectionId && x.IsActive)),
                _ => x => false
            },
            include: x => x.Include(x => x.BookClassRooms).ThenInclude(x => x.ClassRoom).ThenInclude(x => x!.Students),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await BookRules.BookShouldExists(book);

        return book;
    }
}