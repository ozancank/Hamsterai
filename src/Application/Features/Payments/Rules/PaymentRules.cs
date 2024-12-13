using Application.Features.Payments.Models;
using Infrastructure.Payment.Models;
using Infrastructure.Payment.Sipay;

namespace Application.Features.Payments.Rules;

public class PaymentRules(IPaymentDal paymentDal) : IBusinessRule
{
    internal static Task PaymentShouldExists(object payment)
    {
        if (payment == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Payment);
        return Task.CompletedTask;
    }

    internal static Task PaymentShouldExistsAndActive(GetPaymentModel paymentModel)
    {
        PaymentShouldExists(paymentModel);
        if (!paymentModel.IsActive) throw new BusinessException(Strings.DynamicNotFoundOrActive, Strings.Payment);
        return Task.CompletedTask;
    }

    internal static Task PaymentShouldExistsAndActive(Payment payment)
    {
        PaymentShouldExists(payment);
        if (!payment.IsActive) throw new BusinessException(Strings.DynamicNotFoundOrActive, Strings.Payment);
        return Task.CompletedTask;
    }

    internal async Task PaymentShouldExists(Guid id)
    {
        var exists = await paymentDal.IsExistsAsync(x => x.Id == id, enableTracking: false);
        await PaymentShouldExists(exists);
    }

    internal async Task PaymentShouldExistsAndActive(Guid id)
    {
        var payment = await paymentDal.GetAsync(predicate: x => x.Id == id, enableTracking: false);
        await PaymentShouldExistsAndActive(payment);
    }

    internal static Task PaymentDetailPricesShouldEqual(double realAmount, double detailAmount, string packageName)
    {
        if (realAmount != detailAmount)
            throw new BusinessException($"""
                    {packageName} içi fiyat değerleri eşit değil.
                    Gönderilen Fiyat: {detailAmount}
                    Hesaplanan Fiyat: {realAmount}
                    """);
        return Task.CompletedTask;
    }

    internal static Task PaymentTotalricesShouldEqual(double totalPrice, double paymentAmount)
    {
        if (totalPrice != paymentAmount)
        {
            throw new BusinessException($"""
                    Ödeme tutar değerleri eşit değil.
                    Gönderilen Fiyat: {paymentAmount}
                    Hesaplanan Fiyat: {totalPrice}
                    """);
        }
        return Task.CompletedTask;
    }

    internal static Task PaymentControl(GetPaymentResponseModel paymentControl)
    {
        if (paymentControl == null) throw new BusinessException(Strings.DynamicNotVerified, Strings.Payment);
        return Task.CompletedTask;
    }

    internal static Task SipayMerchantKeyControl(string? merchantKey)
    {
        if (merchantKey.IsEmpty() || merchantKey != SipayConfiguration.MerchantKey)
            throw new Exception(Strings.InvalidValue);
        return Task.CompletedTask;
    }

    //internal async Task PaymentNoCanNotBeDuplicated(string no, int? paymentId = null)
    //{
    //    no = no.Trim();
    //    var payment = await paymentDal.GetAsync(predicate: x => x.PaymentNo == no, enableTracking: false);
    //    if (paymentId == null && payment != null) throw new BusinessException(Strings.DynamicExists, no);
    //    if (paymentId != null && payment != null && payment.Id != paymentId) throw new BusinessException(Strings.DynamicExists, no);
    //}

    //internal async Task PaymentEmailCanNotBeDuplicated(string email, int? paymentId = null)
    //{
    //    email = email.Trim();
    //    var payment = await paymentDal.GetAsync(predicate: x => x.Email == email, enableTracking: false);
    //    if (paymentId == null && payment != null) throw new BusinessException(Strings.DynamicExists, email);
    //    if (paymentId != null && payment != null && payment.Id != paymentId) throw new BusinessException(Strings.DynamicExists, email);
    //}

    //internal async Task PaymentPhoneCanNotBeDuplicated(string phone, int? paymentId = null)
    //{
    //    phone = phone.Trim();
    //    var payment = await paymentDal.GetAsync(predicate: x => x.Phone == phone, enableTracking: false);
    //    if (paymentId == null && payment != null) throw new BusinessException(Strings.DynamicExists, phone);
    //    if (paymentId != null && payment != null && payment.Id != paymentId) throw new BusinessException(Strings.DynamicExists, phone);
    //}

    //internal async Task PaymentsShouldExistsAndActiveByIds(List<int> paymentIds)
    //{
    //    var payments = !paymentIds.Except(await paymentDal.Query().AsNoTracking().Where(x => x.IsActive).Select(x => x.Id).ToListAsync()).Any();
    //    if (!payments) throw new BusinessException(Strings.DynamicNotFoundOrActive, Strings.Payment);
    //}
}