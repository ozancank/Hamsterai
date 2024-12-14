using Application.Features.Payments.Models;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Payments.Queries;

public class GetPaymentsByUserIdQuery : IRequest<PageableModel<GetPaymentModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public long UserId { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => true;
}

public class GetPaymentsByUserIdQueryHandler(IMapper mapper,
                                             ICommonService commonService,
                                             IOrderDal orderDal,
                                             IPaymentDal paymentDal) : IRequestHandler<GetPaymentsByUserIdQuery, PageableModel<GetPaymentModel>>
{
    public async Task<PageableModel<GetPaymentModel>> Handle(GetPaymentsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var entities = await paymentDal.GetPageListAsyncAutoMapper<GetPaymentModel>(
            enableTracking: false,
            index: request.PageRequest.Page,
            size: request.PageRequest.PageSize,
            predicate: x => x.UserId == request.UserId && (commonService.IsByPass || commonService.HttpUserType == UserTypes.Administator || x.UserId == commonService.HttpUserId),
            orderBy: x => x.OrderByDescending(x => x.CreateDate),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        await entities.Items.ForEachAsync(async (x) =>
        {
            if (x.PaymentReason == PaymentReason.FirstPayment)
            {
                var orderId = Convert.ToInt32(x.ReasonId ?? "0");
                x.OrderNo = await orderDal.Query().AsNoTracking().Where(o => o.UserId == x.UserId && o.Id == orderId).Select(x => x.OrderNo).FirstOrDefaultAsync(cancellationToken);
            }
            else if (x.PaymentReason == PaymentReason.RenewalPayment)
            {
                var orderId = Convert.ToInt32(x.ReasonId ?? "0");
                x.OrderNo = await orderDal.Query().AsNoTracking().Where(o => o.UserId == x.UserId && o.Id == orderId).Select(x => $"{x.OrderNo} siparişin yinelenen ödemesi").FirstOrDefaultAsync(cancellationToken: cancellationToken);
            }
        });

        var result = mapper.Map<PageableModel<GetPaymentModel>>(entities);
        return result;
    }
}