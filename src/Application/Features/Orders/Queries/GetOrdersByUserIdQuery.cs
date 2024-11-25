using Application.Features.Orders.Models;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Orders.Queries;

public class GetOrdersByUserIdQuery : IRequest<PageableModel<GetOrderModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public long UserId { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => true;
}

public class GetOrdersByUserIdQueryHandler(IMapper mapper,
                                           ICommonService commonService,
                                           IOrderDal orderDal) : IRequestHandler<GetOrdersByUserIdQuery, PageableModel<GetOrderModel>>
{
    public async Task<PageableModel<GetOrderModel>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
    {
        var entities = await orderDal.GetPageListAsyncAutoMapper<GetOrderModel>(
            enableTracking: false,
            predicate: x => x.UserId == request.UserId && (commonService.IsByPass || commonService.HttpUserType == UserTypes.Administator || x.UserId == commonService.HttpUserId),
            include: x => x.Include(u => u.User).ThenInclude(u => u!.PackageUsers).ThenInclude(u => u.Package)
                           .Include(u => u.User).ThenInclude(u => u!.Questions)
                           .Include(u => u.OrderDetails),
            orderBy: x => x.OrderByDescending(x => x.CreateDate),
            index: request.PageRequest.Page,
            size: request.PageRequest.PageSize,
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetOrderModel>>(entities);
        return result;
    }
}