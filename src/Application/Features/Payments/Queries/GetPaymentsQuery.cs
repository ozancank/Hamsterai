using Application.Features.Payments.Models;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Payments.Queries;

public class GetPaymentsQuery : IRequest<PageableModel<GetPaymentModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School, UserTypes.Teacher];
    public bool AllowByPass => false;
}

public class GetPaymentsQueryHandler(IMapper mapper,
                                   ICommonService commonService,
                                   IOrderDal orderDal,
                                   IPaymentDal paymentDal) : IRequestHandler<GetPaymentsQuery, PageableModel<GetPaymentModel>>
{
    public async Task<PageableModel<GetPaymentModel>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();

        var payments = await paymentDal.GetPageListAsyncAutoMapper<GetPaymentModel>(
            enableTracking: false,
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            predicate: x => commonService.HttpUserType == UserTypes.Administator || x.UserId == commonService.HttpUserId,
            orderBy: x => x.OrderByDescending(x => x.CreateDate),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        await payments.Items.ForEachAsync(async (x) =>
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

        var result = mapper.Map<PageableModel<GetPaymentModel>>(payments);
        return result;
    }
}