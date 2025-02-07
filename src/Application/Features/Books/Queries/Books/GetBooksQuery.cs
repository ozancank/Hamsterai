using Application.Features.Books.Models.Books;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Books.Queries.Books;

public class GetBooksQuery : IRequest<PageableModel<GetBookModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }

    public required BookRequestModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School, UserTypes.Teacher, UserTypes.Student];
    public bool AllowByPass => false;
}

public class GetBooksQueryHandler(IMapper mapper,
                                  IBookDal bookDal,
                                  ICommonService commonService) : IRequestHandler<GetBooksQuery, PageableModel<GetBookModel>>
{
    public async Task<PageableModel<GetBookModel>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        var schoolId = commonService.HttpSchoolId;
        var userType = commonService.HttpUserType;
        var connectionId = commonService.HttpConnectionId;

        request.PageRequest ??= new PageRequest();
        request.Model ??= new BookRequestModel();

        var books = await bookDal.GetPageListAsyncAutoMapper<GetBookModel>(
        enableTracking: false,
        predicate: x => x.IsActive
                       && (request.Model.LessonId <= 0 || x.LessonId == request.Model.LessonId)
                       && (userType == UserTypes.Administator
                          || userType != UserTypes.Administator && x.SchoolId == schoolId
                             && (userType != UserTypes.Student || x.BookClassRooms.Any(x => x.ClassRoom != null && x.ClassRoom.Students.Any(x => x.Id == commonService.HttpConnectionId && x.IsActive)))),
        include: x => x.Include(u => u.School)
                       .Include(u => u.Publisher)
                       .Include(u => u.BookClassRooms),
        configurationProvider: mapper.ConfigurationProvider,
        index: request.PageRequest.Page, size: request.PageRequest.PageSize,
        cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetBookModel>>(books);
        return result;
    }
}