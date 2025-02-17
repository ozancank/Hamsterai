using Application.Features.Books.Models.Books;
using Application.Features.Books.Rules;
using Application.Features.Schools.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Books.Commands.Books;

public class UpdateBookClassRoomCommand : IRequest<GetBookModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required UpdateBookClassRoomModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.School];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class UpdateBookClassRoomCommandHandler(IMapper mapper,
                                               ICommonService commonService,
                                               IBookDal bookDal,
                                               IRBookClassRoomDal bookClassRoomDal,
                                               ClassRoomRules classRoomRules,
                                               SchoolRules schoolRules,
                                               BookRules bookRules) : IRequestHandler<UpdateBookClassRoomCommand, GetBookModel>
{
    public async Task<GetBookModel> Handle(UpdateBookClassRoomCommand request, CancellationToken cancellationToken)
    {
        if (commonService.HttpUserType == UserTypes.School) request.Model.SchoolId = commonService.HttpSchoolId ?? 0;

        await bookRules.BookShouldExistsAndActive(request.Model.Id);
        await schoolRules.SchoolShouldExistsAndActive(request.Model.SchoolId);
        await bookRules.BookShouldBeReadyById(request.Model.Id, request.Model.SchoolId);

        var date = DateTime.Now;

        var deleteList = await bookClassRoomDal.GetListAsync(
            predicate: x => x.BookId == request.Model.Id && x.Book != null && x.Book.SchoolId == request.Model.SchoolId,
            include: x => x.Include(u => u.Book),
            cancellationToken: cancellationToken);

        List<RBookClassRoom> bookClassRooms = [];

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
                BookId = request.Model.Id,
                ClassRoomId = classRoomId,
            });
        }

        await bookDal.ExecuteWithTransactionAsync(async () =>
        {
            await bookClassRoomDal.DeleteRangeAsync(deleteList, cancellationToken: cancellationToken);
            await bookClassRoomDal.AddRangeAsync(bookClassRooms, cancellationToken: cancellationToken);
        }, cancellationToken: cancellationToken);

        var result = await bookDal.GetAsyncAutoMapper<GetBookModel>(
            enableTracking: false,
            predicate: x => x.Id == request.Model.Id,
            include: x => x.Include(u => u.BookClassRooms).ThenInclude(x => x.ClassRoom)
                           .Include(u => u.Publisher)
                           .Include(u => u.School),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        return result;
    }
}

public class UpdateBookClassRoomCommandValidator : AbstractValidator<UpdateBookClassRoomCommand>
{
    public UpdateBookClassRoomCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotNull().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.Id).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Book]);

        RuleFor(x => x.Model.SchoolId).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.School]);

        RuleFor(x => x.Model.ClassRoomIds).NotNull().WithMessage(Strings.DynamicNotNull, [Strings.ClassRoom]);
    }
}