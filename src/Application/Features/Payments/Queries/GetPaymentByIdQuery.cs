using Application.Features.Payments.Models;
using Application.Features.Payments.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Payments.Queries;

public class GetPaymentByIdQuery : IRequest<GetPaymentModel?>, ISecuredRequest<UserTypes>
{
    public Guid Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetPaymentByIdQueryHandler(IMapper mapper,
                                        ICommonService commonService,
                                        IOrderDal orderDal,
                                        IPaymentDal paymentDal) : IRequestHandler<GetPaymentByIdQuery, GetPaymentModel?>
{
    public async Task<GetPaymentModel?> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await paymentDal.GetAsyncAutoMapper<GetPaymentModel>(
            enableTracking: request.Tracking,
            predicate: x => x.Id == request.Id && (commonService.HttpUserType == UserTypes.Administator || x.UserId == commonService.HttpUserId),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await PaymentRules.PaymentShouldExists(entity);

        if (entity.PaymentReason == PaymentReason.FirstPayment)
        {
            var orderId = Convert.ToInt32(entity.ReasonId ?? "0");
            entity.OrderNo = await orderDal.Query().AsNoTracking().Where(o => o.UserId == entity.UserId && o.Id == orderId).Select(x => entity.OrderNo).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }

        return entity;
    }
}