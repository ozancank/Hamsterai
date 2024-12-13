using Application.Features.Orders.Constants;
using Application.Features.Orders.Models;
using Application.Features.Orders.Rules;
using Application.Features.Packages.Rules;
using Application.Features.Payments.Rules;
using Application.Features.Users.Rules;
using DataAccess.Abstract.Core;
using Infrastructure.Payment;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Caching;
using OCK.Core.Pipelines.Logging;
using System.Text.Json;

namespace Application.Features.Orders.Commands;

public class AddOrderCommand : IRequest<GetOrderModel>, ISecuredRequest<UserTypes>, ILoggableRequest, ICacheRemoverRequest
{
    public required AddOrderModel Model { get; set; }
    public bool WillSave { get; set; } = true;

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => true;
    public string[] HidePropertyNames { get; } = [];
    public string[] CacheKey { get; } = [$"^{Strings.CacheStatusAndLicence}"];
}

public class AddOrderCommandHandler(IMapper mapper,
                                    IPackageDal packageDal,
                                    IUserDal userDal,
                                    IOrderDal orderDal,
                                    IOrderDetailDal orderDetailDal,
                                    IPaymentDal paymentDal,
                                    IPackageUserDal packageUserDal,
                                    IPaymentSipayDal paymentSipayDal,
                                    IPaymentApi paymentApi) : IRequestHandler<AddOrderCommand, GetOrderModel>
{
    public async Task<GetOrderModel> Handle(AddOrderCommand request, CancellationToken cancellationToken)
    {
        var date = DateTime.Now;
        request.Model.Email = request.Model.Email!.Trim().ToLower();

        var user = await userDal.GetAsync(
            enableTracking: false,
            predicate: x => x.Email == request.Model.Email,
            cancellationToken: cancellationToken);
        await UserRules.UserShouldExists(user);

        var orderCount = await orderDal.CountOfRecordAsync(
            enableTracking: false,
            predicate: x => x.UserId == user.Id,
            cancellationToken: cancellationToken);
        var order = new Order
        {
            Id = await orderDal.GetNextPrimaryKeyAsync(x => x.Id, cancellationToken: cancellationToken),
            IsActive = true,
            CreateDate = date,
            CreateUser = 1,
            UpdateDate = date,
            UpdateUser = 1,
            UserId = user.Id,
            OrderNo = $"{date:yyMMdd}-{user.Id}-{orderCount + 1}",
        };

        var orderDetail = new List<OrderDetail>();
        var packageUsers = new List<(bool, PackageUser)>();
        var subTotal = 0.0;

        var packages = await packageDal.GetListAsync(
            enableTracking: false,
            predicate: x => request.Model.OrderDetails.Select(x => x.PackageId).Contains(x.Id),
            orderBy: x => x.OrderBy(x => x.Type),
            cancellationToken: cancellationToken);

        var packageUserList = await packageUserDal.GetListAsync(
            predicate: x => x.UserId == user.Id && x.Package != null,
            orderBy: x => x.OrderBy(x => x.Package != null ? x.Package.Type : default),
            cancellationToken: cancellationToken);

        foreach (var detail in request.Model.OrderDetails)
        {
            var package = packages.FirstOrDefault(x => x.Id == detail.PackageId);
            await PackageRules.PackageShouldExistsAndActive(package);

            var quantity = detail.Quantity;
            var price = quantity * package!.UnitPrice;
            subTotal += price;
            var discountRatio = detail.DiscountRatio;
            var discountAmount = (price * discountRatio / 100.0).RoundDouble();
            var taxBase = (price - discountAmount).RoundDouble();
            var taxRatio = package.TaxRatio;
            var taxAmount = (taxBase * taxRatio / 100.0).RoundDouble();
            var realAmount = (taxBase + taxAmount).RoundDouble();

            await OrderRules.OrderDetailPricesShouldEqual(realAmount, detail.Amount, package.Name!);

            var ordDtl = new OrderDetail
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                CreateDate = date,
                CreateUser = 1,
                UpdateDate = date,
                UpdateUser = 1,
                OrderId = order.Id,
                PackageId = package.Id,
                QuestionCredit = quantity * package.QuestionCredit,
                Quantity = detail.Quantity,
                UnitPrice = package.UnitPrice,
                DiscountRatio = discountRatio,
                DiscountAmount = discountAmount,
                TaxBase = taxBase,
                TaxRatio = package.TaxRatio,
                TaxAmount = taxAmount,
                Amount = realAmount,
            };
            orderDetail.Add(ordDtl);

            var packageUser = packageUserList.FirstOrDefault(x => x.PackageId == package.Id && x.UserId == user.Id) ?? new PackageUser();
            var packageUserIsExists = packageUser.Id != Guid.Empty;

            if (package.Type == PackageType.Additional)
                PackageRules.AdditionalPackageShouldBeWithBasePackage(date, packageUsers, packages, packageUserList);

            if (packageUserIsExists)
            {
                packageUser.UpdateUser = 1;
                packageUser.UpdateDate = date;
                packageUser.RenewCount++;
                packageUser.QuestionCredit = packageUser.EndDate.AddDays(1).AddSeconds(-1) <= date ? 0 : packageUser.QuestionCredit + package.QuestionCredit;
                packageUser.EndDate = packageUser.EndDate <= date ? date : packageUser.EndDate;
            }
            else
            {
                packageUser = new PackageUser
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    CreateUser = 1,
                    CreateDate = date,
                    UpdateUser = 1,
                    UpdateDate = date,
                    PackageId = package.Id,
                    UserId = user.Id,
                    RenewCount = 1,
                    EndDate = date,
                    QuestionCredit = package.QuestionCredit,
                };
            }

            switch (package.PaymentRenewalPeriod)
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

            packageUsers.Add((packageUserIsExists, packageUser));
        }

        var totalDiscount = orderDetail.Sum(x => x.DiscountAmount);
        var totalTaxBase = subTotal - totalDiscount;
        var totalTaxAmount = orderDetail.Sum(x => x.TaxAmount);
        var totalPrice = (totalTaxBase + totalTaxAmount).RoundDouble();

        foreach (var property in OrderStatics.AddPaymentSipayModelForAddOrderModelStringProperties)
        {
            var value = property.GetValue(request.Model.Payment) as string;
            if (value.IsNotEmpty()) property.SetValue(request.Model.Payment, value?.UrlDecode());
        }

        await OrderRules.OrderTotalricesShouldEqual(totalPrice, request.Model.Payment!.Amount);

        var paymentSipay = mapper.Map<PaymentSipay>(request.Model.Payment);
        paymentSipay.Id = Guid.NewGuid();
        paymentSipay.IsActive = true;
        paymentSipay.CreateUser = 1;
        paymentSipay.CreateDate = date;
        paymentSipay.UpdateUser = 1;
        paymentSipay.UpdateDate = date;
        paymentSipay.UserId = user.Id;

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            IsActive = true,
            CreateDate = date,
            CreateUser = 1,
            UpdateDate = date,
            UpdateUser = 1,
            UserId = user.Id,
            PaymentDate = date,
            PaymentReason = PaymentReason.FirstPayment,
            ReasonId = order.Id.ToString(),
            Amount = totalPrice,
            PaymentSipayId = paymentSipay.Id
        };

        order.SubTotal = subTotal;
        order.DiscountAmount = totalDiscount;
        order.TaxBase = totalTaxBase;
        order.TaxAmount = totalTaxAmount;
        order.Amount = totalPrice;

        user.IsActive = true;

        var result = mapper.Map<GetOrderModel>(order);
        if (request.WillSave)
        {
            var paymentControl = await paymentApi.GetPayment(paymentSipay.InvoiceId!, payment.Amount);
            Console.WriteLine(JsonSerializer.Serialize(paymentControl));
            await PaymentRules.PaymentControl(paymentControl);

            paymentSipay.BankStatusCode = paymentControl.BankStatusCode;
            paymentSipay.BankStatusDescription = paymentControl.BankStatusDescription;
            paymentSipay.TransactionStatus = paymentControl.TransactionStatus;
            paymentSipay.TransactionId = paymentControl.TransactionId;
            paymentSipay.Message = paymentControl.Message;
            paymentSipay.Reason = paymentControl.Reason;
            paymentSipay.TotalRefundedAmount = paymentControl.TotalRefundedAmount;
            paymentSipay.ProductPrice = paymentControl.ProductPrice;
            paymentSipay.TransactionAmount = paymentControl.TransactionAmount;
            paymentSipay.RecurringId = paymentControl.RecurringId;
            paymentSipay.RefNumber = paymentControl.RefNumber;
            paymentSipay.RecurringPlanCode = paymentControl.RecurringPlanCode;
            paymentSipay.RecurringNumber = 1;
            paymentSipay.ActionDate = paymentControl.SettlementDate;
            paymentSipay.NextActionDate = paymentControl.NextActionDate;
            paymentSipay.RecurringStatus = paymentControl.RecurringStatus;
            paymentSipay.SettlementDate = paymentControl.SettlementDate;

            if (!user.AutomaticPayment)
            {
                var plan = await paymentApi.GetRequrring(paymentSipay.RecurringPlanCode!, paymentSipay.RecurringNumber);
                if (plan.RecurringStatus?.Equals("Active", StringComparison.OrdinalIgnoreCase) ?? false)
                    await paymentApi.UpdateRecurringRequest(new()
                    {
                        PlanCode = paymentSipay.RecurringPlanCode!,
                        RecurringActive = false,
                    });
            }

            result = await orderDal.ExecuteWithTransactionAsync(async () =>
            {
                await userDal.UpdateAsync(user, cancellationToken: cancellationToken);
                await orderDal.AddAsync(order, cancellationToken: cancellationToken);
                await orderDetailDal.AddRangeAsync(orderDetail, cancellationToken: cancellationToken);
                await paymentSipayDal.AddAsync(paymentSipay, cancellationToken: cancellationToken);
                await paymentDal.AddAsync(payment, cancellationToken: cancellationToken);

                foreach (var packageUser in packageUsers)
                {
                    if (packageUser.Item1)
                        await packageUserDal.UpdateAsync(packageUser.Item2, cancellationToken: cancellationToken);
                    else
                        await packageUserDal.AddAsync(packageUser.Item2, cancellationToken: cancellationToken);
                }

                return await orderDal.GetAsyncAutoMapper<GetOrderModel>(
                    predicate: x => x.Id == order.Id,
                    include: x => x.Include(u => u.User).ThenInclude(u => u!.PackageUsers).ThenInclude(u => u.Package)
                                   .Include(u => u.User).ThenInclude(u => u!.Questions)
                                   .Include(u => u.OrderDetails).ThenInclude(u => u.Package),
                    configurationProvider: mapper.ConfigurationProvider,
                    cancellationToken: cancellationToken);
            }, cancellationToken: cancellationToken);
        }

        return result;
    }
}

public class AddOrderCommandValidator : AbstractValidator<AddOrderCommand>
{
    public AddOrderCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotNull().WithMessage(Strings.InvalidValue);

        //RuleFor(x => x.Model.User).NotNull().WithMessage(Strings.InvalidValue);

        //RuleFor(x => x.Model.User!.Name).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        //RuleFor(x => x.Model.User!.Name).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        //RuleFor(x => x.Model.User!.Name).MaximumLength(250).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "250"]);

        //RuleFor(x => x.Model.User!.Surname).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Surname]);
        //RuleFor(x => x.Model.User!.Surname).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Surname, "2"]);
        //RuleFor(x => x.Model.User!.Surname).MaximumLength(250).WithMessage(Strings.DynamicMaxLength, [Strings.Surname, "100"]);

        RuleFor(x => x.Model.Email).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [$"{Strings.Authorized} {Strings.OfEmail}"]);
        RuleFor(x => x.Model.Email).MinimumLength(5).WithMessage(Strings.DynamicMinLength, [$"{Strings.Authorized} {Strings.OfEmail}", "5"]);
        RuleFor(x => x.Model.Email).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [$"{Strings.Authorized} {Strings.OfEmail}", "100"]);
        RuleFor(x => x.Model.Email).EmailAddress().WithMessage(Strings.EmailWrongFormat);

        //RuleFor(x => x.Model.User!.Password)
        //    .NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Password])
        //    .MinimumLength(8).WithMessage(Strings.DynamicMinLength, [Strings.Password, "8"])
        //    .Matches("[a-zA-Z]").WithMessage(Strings.PasswordLetter)
        //    .Matches("[0-9]").WithMessage(Strings.PasswordNumber);

        //RuleFor(x => x.Model.User!.Phone).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [$"{Strings.Authorized} {Strings.OfPhone}"]);
        //RuleFor(x => x.Model.User!.Phone).MinimumLength(10).WithMessage(Strings.DynamicMinLength, [$"{Strings.Authorized} {Strings.OfPhone}", "10"]);
        //RuleFor(x => x.Model.User!.Phone).MaximumLength(15).WithMessage(Strings.DynamicMaxLength, [$"{Strings.Authorized} {Strings.OfPhone}", "15"]);
        //RuleFor(x => x.Model.User!.Phone).Must(x => double.TryParse(x, out _)).WithMessage(Strings.DynamicOnlyDigit, [$"{Strings.Authorized} {Strings.OfPhone}"]);

        //RuleFor(x => x.Model.User!.TaxNumber).MaximumLength(11).WithMessage(Strings.DynamicMaxLength, [$"{Strings.TaxNumber}/{Strings.Identity} {Strings.No}", "11"]);

        RuleFor(x => x.Model.OrderDetails).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Detail]);
        RuleForEach(x => x.Model.OrderDetails).SetValidator(new AddOrderDetailForAddOrderModelValidator());

        RuleFor(x => x.Model.Payment).NotNull().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.Payment!.Amount).GreaterThan(0.0).WithMessage(Strings.DynamicGratherThan, [Strings.Amount, "0"]);
    }
}

public class AddOrderDetailForAddOrderModelValidator : AbstractValidator<AddOrderDetailForAddOrderModel>
{
    public AddOrderDetailForAddOrderModelValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.PackageId).GreaterThan((short)0).WithMessage(Strings.DynamicGratherThan, [Strings.Package, "0"]);

        RuleFor(x => x.Quantity).GreaterThan((byte)0).WithMessage(Strings.DynamicGratherThan, [Strings.Quantity, "0"]);

        RuleFor(x => x.DiscountRatio).InclusiveBetween(0.0, 100.0).WithMessage(Strings.DynamicBetween, [$"{Strings.Discount} {Strings.OfRatio}", "0", "100"]);

        RuleFor(x => x.Amount).GreaterThan(0.0).WithMessage(Strings.DynamicGratherThan, [Strings.Amount, "0"]);
    }
}