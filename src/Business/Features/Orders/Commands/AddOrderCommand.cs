using Business.Features.Orders.Models;
using Business.Features.Orders.Rules;
using Business.Features.Packages.Rules;
using Business.Features.Users.Rules;
using DataAccess.Abstract.Core;
using Domain.Entities.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Caching;
using OCK.Core.Pipelines.Logging;
using OCK.Core.Security.HashingHelper;

namespace Business.Features.Orders.Commands;

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
                                    //ICommonService commonService,
                                    UserRules userRules) : IRequestHandler<AddOrderCommand, GetOrderModel>
{
    public async Task<GetOrderModel> Handle(AddOrderCommand request, CancellationToken cancellationToken)
    {
        var date = DateTime.Now;
        request.Model.User!.Email = request.Model.User.Email!.Trim().ToLower();
        request.Model.User.Phone = request.Model.User.Phone?.TrimForPhone();

        await userRules.UserNameCanNotBeDuplicated(request.Model.User!.Email!);
        await userRules.UserEmailCanNotBeDuplicated(request.Model.User.Email!);
        await userRules.UserPhoneCanNotBeDuplicated(request.Model.User.Phone!);

        HashingHelper.CreatePasswordHash(request.Model.User.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = await userDal.GetAsync(
            enableTracking: false,
            predicate: x => x.Email == request.Model.User.Email,
            cancellationToken: cancellationToken) ?? new User();

        if (user.Id <= 0)
        {
            user.Id = await userDal.GetNextPrimaryKeyAsync(x => x.Id, cancellationToken: cancellationToken);
            user.IsActive = false;
            user.CreateDate = date;
            user.UserName = request.Model.User.Email;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.MustPasswordChange = false;
            user.Name = request.Model.User.Name;
            user.Surname = request.Model.User.Surname;
            user.Phone = request.Model.User.Phone;
            user.ProfileUrl = string.Empty;
            user.Email = request.Model.User.Email;
            user.Type = UserTypes.Person;
            user.SchoolId = null;
            user.ConnectionId = null;
            user.PackageCredit = 0;
            user.AddtionalCredit = 0;
            user.AutomaticPayment = request.Model.User.AutomaticPayment;
            user.TaxNumber = request.Model.User.TaxNumber;
            user.LicenceEndDate = date;
        }

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
        var packageUsers = new List<PackageUser>();
        var subTotal = 0.0;
        foreach (var detail in request.Model.OrderDetails)
        {
            var package = await packageDal.GetAsync(
                enableTracking: false,
                predicate: x => x.Id == detail.PackageId,
                cancellationToken: cancellationToken);
            await PackageRules.PackageShouldExistsAndActive(package);

            var quantity = detail.Quantity;
            var price = quantity * package.UnitPrice;
            subTotal += price;
            var discountRatio = detail.DiscountRatio;
            var discountAmount = Math.Round(price * discountRatio / 100.0, 2);
            var taxBase = price - discountAmount;
            var taxRatio = package.TaxRatio;
            var taxAmount = Math.Round(taxBase * taxRatio / 100.0, 2);
            var realAmount = Math.Round(taxBase + taxAmount, 2);

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

            switch (package.PaymentRenewalPeriod)
            {
                case PaymentRenewalPeriod.Daily:
                    user.LicenceEndDate = user.LicenceEndDate.AddDays(1);
                    break;

                case PaymentRenewalPeriod.Weekly:
                    user.LicenceEndDate = user.LicenceEndDate.AddDays(7);
                    break;

                case PaymentRenewalPeriod.Monthly:
                    user.LicenceEndDate = user.LicenceEndDate.AddMonths(1);
                    break;

                case PaymentRenewalPeriod.Quarterly:
                    user.LicenceEndDate = user.LicenceEndDate.AddMonths(3);
                    break;

                case PaymentRenewalPeriod.SemiAnnually:
                    user.LicenceEndDate = user.LicenceEndDate.AddMonths(6);
                    break;

                case PaymentRenewalPeriod.Annually:
                    user.LicenceEndDate = user.LicenceEndDate.AddYears(1);
                    break;

                case PaymentRenewalPeriod.None:
                default:
                    break;
            }

            switch (package.Type)
            {
                case PackageType.Base:
                    user.PackageCredit += package.QuestionCredit;
                    break;

                case PackageType.Additional:
                    user.AddtionalCredit += package.QuestionCredit;
                    break;

                case PackageType.None:
                default:
                    break;
            }

            var packageUser = await packageUserDal.GetAsync(
                enableTracking: false,
                predicate: x => x.PackageId == package.Id && x.UserId == user.Id,
                cancellationToken: cancellationToken) ?? new PackageUser();

            if (packageUser != null)
            {
                packageUser.UpdateUser = 1;
                packageUser.UpdateDate = date;
                packageUser.RenewCount++;
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
                    RenewCount = 0,
                };
            }

            packageUsers.Add(packageUser);
        }

        var totalDiscount = orderDetail.Sum(x => x.DiscountAmount);
        var totalTaxBase = subTotal - totalDiscount;
        var totalTaxAmount = orderDetail.Sum(x => x.TaxAmount);
        var totalPrice = Math.Round(totalTaxBase + totalTaxAmount, 2);

        await OrderRules.OrderTotalricesShouldEqual(totalPrice, request.Model.Payment!.Amount);

        var payment = new Payment
        {
            Id = new Guid(),
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
            SipayMerchantKey = request.Model.Payment.SipayMerchantKey,
            SipayPlanCode = request.Model.Payment.SipayPlanCode,
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
            result = await orderDal.ExecuteWithTransactionAsync(async () =>
            {
                if (user.Id <= 0)
                    await userDal.AddAsync(user, cancellationToken: cancellationToken);
                else
                    await userDal.UpdateAsync(user, cancellationToken: cancellationToken);

                await orderDal.AddAsync(order, cancellationToken: cancellationToken);
                await orderDetailDal.AddRangeAsync(orderDetail, cancellationToken: cancellationToken);
                await paymentDal.AddAsync(payment, cancellationToken: cancellationToken);

                foreach (var packageUser in packageUsers)
                {
                    if (packageUser.CreateDate == date)
                        await packageUserDal.AddAsync(packageUser, cancellationToken: cancellationToken);
                    else
                        await packageUserDal.UpdateAsync(packageUser, cancellationToken: cancellationToken);
                }

                return await orderDal.GetAsyncAutoMapper<GetOrderModel>(
                    predicate: x => x.Id == order.Id,
                    include: x => x.Include(u => u.User).ThenInclude(u => u!.PackageUsers).ThenInclude(u => u.Package)
                                   .Include(u => u.OrderDetails),
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

        RuleFor(x => x.Model.User).NotNull().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.User!.Name).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Model.User!.Name).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Model.User!.Name).MaximumLength(250).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "250"]);

        RuleFor(x => x.Model.User!.Surname).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Surname]);
        RuleFor(x => x.Model.User!.Surname).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Surname, "2"]);
        RuleFor(x => x.Model.User!.Surname).MaximumLength(250).WithMessage(Strings.DynamicMaxLength, [Strings.Surname, "100"]);

        RuleFor(x => x.Model.User!.Email).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [$"{Strings.Authorized} {Strings.OfEmail}"]);
        RuleFor(x => x.Model.User!.Email).MinimumLength(5).WithMessage(Strings.DynamicMinLength, [$"{Strings.Authorized} {Strings.OfEmail}", "5"]);
        RuleFor(x => x.Model.User!.Email).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [$"{Strings.Authorized} {Strings.OfEmail}", "100"]);
        RuleFor(x => x.Model.User!.Email).EmailAddress().WithMessage(Strings.EmailWrongFormat);

        RuleFor(x => x.Model.User!.Password)
            .NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Password])
            .MinimumLength(8).WithMessage(Strings.DynamicMinLength, [Strings.Password, "8"])
            .Matches("[a-zA-Z]").WithMessage(Strings.PasswordLetter)
            .Matches("[0-9]").WithMessage(Strings.PasswordNumber);

        RuleFor(x => x.Model.User!.Phone).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [$"{Strings.Authorized} {Strings.OfPhone}"]);
        RuleFor(x => x.Model.User!.Phone).MinimumLength(10).WithMessage(Strings.DynamicMinLength, [$"{Strings.Authorized} {Strings.OfPhone}", "10"]);
        RuleFor(x => x.Model.User!.Phone).MaximumLength(15).WithMessage(Strings.DynamicMaxLength, [$"{Strings.Authorized} {Strings.OfPhone}", "15"]);
        RuleFor(x => x.Model.User!.Phone).Must(x => double.TryParse(x, out _)).WithMessage(Strings.DynamicOnlyDigit, [$"{Strings.Authorized} {Strings.OfPhone}"]);

        RuleFor(x => x.Model.User!.TaxNumber).MaximumLength(11).WithMessage(Strings.DynamicMaxLength, [$"{Strings.TaxNumber}/{Strings.Identity} {Strings.No}", "11"]);

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