using Application.Features.Books.Models.Books;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Books.Queries.Books;

public class GetBooksByDynamicQuery : IRequest<PageableModel<GetBookModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required Dynamic Dynamic { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School, UserTypes.Teacher, UserTypes.Student];
    public bool AllowByPass => false;
}

public class GetBooksByDynamicQueryHandler(IMapper mapper,
                                               IBookDal bookDal,
                                               ICommonService commonService) : IRequestHandler<GetBooksByDynamicQuery, PageableModel<GetBookModel>>
{
    public async Task<PageableModel<GetBookModel>> Handle(GetBooksByDynamicQuery request, CancellationToken cancellationToken)
    {
        var schoolId = commonService.HttpSchoolId;
        var userType = commonService.HttpUserType;
        var connectionId = commonService.HttpConnectionId;

        request.PageRequest ??= new PageRequest();
        request.Dynamic ??= new Dynamic();

        var books = await bookDal.GetPageListAsyncAutoMapperByDynamic<GetBookModel>(
        dynamic: request.Dynamic,
            predicate: x => userType == UserTypes.Administator
                            || userType != UserTypes.Administator && x.SchoolId == schoolId
                            && (userType != UserTypes.Student || x.BookClassRooms.Any(x => x.ClassRoom != null && x.ClassRoom.Students.Any(x => x.Id == commonService.HttpConnectionId && x.IsActive))),
            enableTracking: false,
            include: x => x.Include(u => u.School)
                       .Include(u => u.Publisher)
                       .Include(u => u.BookClassRooms),
            defaultOrderColumnName: x => x.CreateDate,
            defaultDescending: true,
            configurationProvider: mapper.ConfigurationProvider,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetBookModel>>(books);
        return result;
    }
}