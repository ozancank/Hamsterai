using Application.Features.Orders.Models;
using Application.Features.Orders.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Orders.Queries;

public class GetOrderByIdQuery : IRequest<GetOrderModel?>, ISecuredRequest<UserTypes>
{
    public int OrderId { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetOrderByIdQueryHandler(IMapper mapper,
                                      ICommonService commonService,
                                      IOrderDal orderDal) : IRequestHandler<GetOrderByIdQuery, GetOrderModel?>
{
    public async Task<GetOrderModel?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await orderDal.GetAsyncAutoMapper<GetOrderModel>(
            enableTracking: request.Tracking,
            predicate: x => x.Id == request.OrderId && (commonService.HttpUserType == UserTypes.Administator || x.UserId == commonService.HttpUserId),
            include: x => x.Include(u => u.User).ThenInclude(u => u!.PackageUsers).ThenInclude(u => u.Package)
                           .Include(u => u.User).ThenInclude(u => u!.Questions)
                           .Include(u => u.OrderDetails),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await OrderRules.OrderShouldExists(entity);
        return entity;
    }
}