using Application.Features.Orders.Rules;
using Application.Features.Payments.Rules;
using Infrastructure.Payment.Sipay.Models;
using MediatR;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Payments.Commands;

public class AddPaymentSipayCommand : IRequest, ILoggableRequest
{
    public required SipayRecurringWebHookRequestModel Model { get; set; }

    public string[] HidePropertyNames { get; } = [];
}

public class AddPaymentSipayCommandHandler(IOrderDal orderDal,
                                           IPackageUserDal packageUserDal,
                                           IPaymentDal paymentDal,
                                           IPaymentSipayDal paymentSipayDal) : IRequestHandler<AddPaymentSipayCommand>
{
    public async Task Handle(AddPaymentSipayCommand request, CancellationToken cancellationToken)
    {
        var date = DateTime.Now;

        await PaymentRules.SipayMerchantKeyControl(request.Model.MerchantKey);

        var sipayPayment = await paymentSipayDal.GetAsync(
            enableTracking: false,
            predicate: x => x.RecurringPlanCode == request.Model.PlanCode,
            orderBy: x => x.OrderByDescending(o => o.CreateDate),
            cancellationToken: cancellationToken);
        await PaymentRules.PaymentShouldExists(sipayPayment);

        var payment = await paymentDal.GetAsync(
            enableTracking: false,
            predicate: x => x.IsActive
                            && x.PaymentSipayId == sipayPayment.Id
                            && x.UserId == sipayPayment.UserId,
            cancellationToken: cancellationToken);
        await PaymentRules.PaymentShouldExists(payment);

        var order = await orderDal.GetAsync(
            enableTracking: false,
            predicate: x => x.IsActive
                            && x.UserId == payment.UserId
                            && x.Id == Convert.ToInt32(payment.ReasonId),
            include: x => x.Include(u => u.OrderDetails).ThenInclude(u => u.Package)
                           .Include(u => u.User),
            cancellationToken: cancellationToken);
        await OrderRules.OrderShouldExists(order);

        if (order.OrderDetails == null || order.OrderDetails.Count == 0) return;

        var packageIds = order.OrderDetails.Select(x => x.PackageId).ToList();

        var packageUsers = await packageUserDal.GetListAsync(
            enableTracking: false,
            predicate: x => x.IsActive
                            && x.UserId == payment.UserId
                            && packageIds.Contains(x.PackageId),
            cancellationToken: cancellationToken);

        if (packageUsers.Count == 0) return;

        if (request.Model.Status != "Completed") return;

        var newSipayPayment = new PaymentSipay
        {
            Id = Guid.NewGuid(),
            IsActive = true,
            CreateUser = 1,
            CreateDate = date,
            UpdateUser = 1,
            UpdateDate = date,
            UserId = payment.UserId,
            OrderId = request.Model.OrderId,
            OrderNo = request.Model.OrderId,
            InvoiceId = request.Model.InvoiceId,
            Amount = request.Model.ProductPrice,
            Status = request.Model.Status,
            RecurringNumber = Convert.ToInt32(request.Model.RecurringNumber ?? "0"),
            RecurringPlanCode = request.Model.PlanCode,
            ActionDate = request.Model.ActionDate,
        };

        var newPayment = new Payment
        {
            Id = Guid.NewGuid(),
            IsActive = true,
            CreateUser = 1,
            CreateDate = date,
            UpdateUser = 1,
            UpdateDate = date,
            UserId = payment.UserId,
            Amount = Convert.ToDouble(request.Model.ProductPrice),
            PaymentDate = date,
            PaymentReason = PaymentReason.RenewalPayment,
            ReasonId = payment.ReasonId,
            PaymentSipayId = newSipayPayment.Id,
        };

        packageUsers.ForEach(packageUser =>
        {
            packageUser.IsActive = true;
            packageUser.UpdateUser = 1;
            packageUser.UpdateDate = date;
            packageUser.RenewCount = Convert.ToInt32(request.Model.RecurringNumber ?? "0");
            var orderDetail = order.OrderDetails.First(x => x.PackageId == packageUser.PackageId);
            packageUser.QuestionCredit = packageUser.EndDate.AddDays(1).AddSeconds(-1) <= date ? 0 : packageUser.QuestionCredit + orderDetail.QuestionCredit;

            switch (orderDetail.Package!.PaymentRenewalPeriod)
            {
                case PaymentRenewalPeriod.Daily:
                    packageUser.EndDate = packageUser.EndDate.AddDays(1);
                    break;

                case PaymentRenewalPeriod.Weekly:
                    packageUser.EndDate = packageUser.EndDate.AddDays(7);
                    break;

                case PaymentRenewalPeriod.Monthly:
                    packageUser.EndDate = packageUser.EndDate.AddMonths(1);
                    break;

                case PaymentRenewalPeriod.Quarterly:
                    packageUser.EndDate = packageUser.EndDate.AddMonths(3);
                    break;

                case PaymentRenewalPeriod.SemiAnnually:
                    packageUser.EndDate = packageUser.EndDate.AddMonths(6);
                    break;

                case PaymentRenewalPeriod.Annually:
                    packageUser.EndDate = packageUser.EndDate.AddYears(1);
                    break;

                case PaymentRenewalPeriod.None:
                default:
                    break;
            }
        });

        await paymentDal.ExecuteWithTransactionAsync(async () =>
        {
            await paymentSipayDal.AddAsync(newSipayPayment, cancellationToken: cancellationToken);
            await paymentDal.AddAsync(newPayment, cancellationToken: cancellationToken);
            await packageUserDal.UpdateRangeAsync(packageUsers, cancellationToken: cancellationToken);
        }, cancellationToken: cancellationToken);
    }
}