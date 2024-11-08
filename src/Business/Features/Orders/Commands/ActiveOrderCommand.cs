//using Business.Features.Orders.Rules;
//using Business.Features.Users.Rules;
//using Business.Services.CommonService;
//using DataAccess.Abstract.Core;
//using MediatR;
//using OCK.Core.Pipelines.Authorization;
//using OCK.Core.Pipelines.Caching;
//using OCK.Core.Pipelines.Logging;

//namespace Business.Features.Orders.Commands;

//public class ActiveOrderCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest, ICacheRemoverRequest
//{
//    public int Id { get; set; }

//    public UserTypes[] Roles { get; } = [UserTypes.Administator];
//    public bool AllowByPass => false;
//    public string[] HidePropertyNames { get; } = [];
//    public string[] CacheKey { get; } = [];
//}

//public class ActiveOrderCommandHandler(IOrderDal orderDal,
//                                       IUserDal userDal,
//                                       ICommonService commonService) : IRequestHandler<ActiveOrderCommand, bool>
//{
//    public async Task<bool> Handle(ActiveOrderCommand request, CancellationToken cancellationToken)
//    {
//        var order = await orderDal.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
//        await OrderRules.OrderShouldExists(order);

//        var user = await userDal.GetAsync(x => x.ConnectionId == order.Id, cancellationToken: cancellationToken);
//        await UserRules.UserShouldExists(user);

//        order.UpdateUser = commonService.HttpUserId;
//        order.UpdateDate = DateTime.Now;
//        order.IsActive = true;

//        user.IsActive = true;

//        await orderDal.ExecuteWithTransactionAsync(async () =>
//        {
//            await orderDal.UpdateAsync(order, cancellationToken: cancellationToken);
//            await userDal.UpdateAsync(user, cancellationToken: cancellationToken);
//        }, cancellationToken: cancellationToken);

//        return true;
//    }
//}