using Application.Features.Orders.Models;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Orders.Queries;

public class GetOrdersByDynamicQuery : IRequest<PageableModel<GetOrderModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required Dynamic Dynamic { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetOrdersByDynamicQueryHandler(IMapper mapper,
                                            ICommonService commonService,
                                            IOrderDal orderDal) : IRequestHandler<GetOrdersByDynamicQuery, PageableModel<GetOrderModel>>
{
    public async Task<PageableModel<GetOrderModel>> Handle(GetOrdersByDynamicQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.Dynamic ??= new Dynamic();
        var schoolId = commonService.HttpSchoolId ?? 0;

        var orders = await orderDal.GetPageListAsyncAutoMapperByDynamic<GetOrderModel>(
            dynamic: request.Dynamic,
            defaultOrderColumnName: x => x.CreateDate,
            enableTracking: false,
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            predicate: x => commonService.HttpUserType == UserTypes.Administator || x.UserId == commonService.HttpUserId,
            include: x => x.Include(u => u.User).ThenInclude(u => u!.PackageUsers).ThenInclude(u => u.Package)
                           .Include(u => u.OrderDetails),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetOrderModel>>(orders);
        return result;
    }
}