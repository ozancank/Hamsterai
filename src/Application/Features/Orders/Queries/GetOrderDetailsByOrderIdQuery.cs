using Application.Features.Orders.Models;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Orders.Queries;

public class GetOrderDetailsByOrderIdQuery : IRequest<List<GetOrderDetailModel>>, ISecuredRequest<UserTypes>
{
    public int OrderId { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => true;
}

public class GetOrderDetailsByOrderIdQueryHandler(IMapper mapper,
                                                  ICommonService commonService,
                                                  IOrderDetailDal orderDetailDal) : IRequestHandler<GetOrderDetailsByOrderIdQuery, List<GetOrderDetailModel>>
{
    public async Task<List<GetOrderDetailModel>> Handle(GetOrderDetailsByOrderIdQuery request, CancellationToken cancellationToken)
    {
        var entities = await orderDetailDal.GetListAsyncAutoMapper<GetOrderDetailModel>(
            enableTracking: false,
            predicate: x => x.OrderId == request.OrderId && (commonService.IsByPass || commonService.HttpUserType == UserTypes.Administator || x.Order!.UserId == commonService.HttpUserId),
            include: x => x.Include(u => u.Order).Include(u => u.Package),
            orderBy: x => x.OrderByDescending(x => x.CreateDate),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        return entities;
    }
}