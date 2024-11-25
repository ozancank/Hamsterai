using Application.Features.Orders.Models;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Orders.Queries;

public class GetOrdersQuery : IRequest<PageableModel<GetOrderModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School, UserTypes.Teacher];
    public bool AllowByPass => false;
}

public class GetOrdersQueryHandler(IMapper mapper,
                                   ICommonService commonService,
                                   IOrderDal orderDal) : IRequestHandler<GetOrdersQuery, PageableModel<GetOrderModel>>
{
    public async Task<PageableModel<GetOrderModel>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();

        var orders = await orderDal.GetPageListAsyncAutoMapper<GetOrderModel>(
            enableTracking: false,
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            predicate: x => commonService.HttpUserType == UserTypes.Administator || x.UserId == commonService.HttpUserId,
            include: x => x.Include(u => u.User).ThenInclude(u => u!.PackageUsers).ThenInclude(u => u.Package)
                           .Include(u => u.User).ThenInclude(u => u!.Questions)
                           .Include(u => u.OrderDetails),
            orderBy: x => x.OrderByDescending(x => x.CreateDate),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetOrderModel>>(orders);
        return result;
    }
}