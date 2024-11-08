using Business.Features.Orders.Models;

namespace Business.Features.Orders.Rules;

public class OrderRules(IOrderDal orderDal) : IBusinessRule
{
    internal static Task OrderShouldExists(object order)
    {
        if (order == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Order);
        return Task.CompletedTask;
    }

    internal static Task OrderShouldExistsAndActive(GetOrderModel orderModel)
    {
        OrderShouldExists(orderModel);
        if (!orderModel.IsActive) throw new BusinessException(Strings.DynamicNotFoundOrActive, Strings.Order);
        return Task.CompletedTask;
    }

    internal static Task OrderShouldExistsAndActive(Order order)
    {
        OrderShouldExists(order);
        if (!order.IsActive) throw new BusinessException(Strings.DynamicNotFoundOrActive, Strings.Order);
        return Task.CompletedTask;
    }

    internal async Task OrderShouldExists(int id)
    {
        var exists = await orderDal.IsExistsAsync(x => x.Id == id, enableTracking: false);
        await OrderShouldExists(exists);
    }

    internal async Task OrderShouldExistsAndActive(int id)
    {
        var order = await orderDal.GetAsync(predicate: x => x.Id == id, enableTracking: false);
        await OrderShouldExistsAndActive(order);
    }

    internal static Task OrderDetailPricesShouldEqual(double realAmount, double detailAmount, string packageName)
    {
        if (realAmount != detailAmount)
            throw new BusinessException($"""
                    {packageName} içi fiyat değerleri eşit değil.
                    Gönderilen Fiyat: {detailAmount}
                    Hesaplanan Fiyat: {realAmount}
                    """);
        return Task.CompletedTask;
    }

    internal static Task OrderTotalricesShouldEqual(double totalPrice, double paymentAmount)
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

    //internal async Task OrderNoCanNotBeDuplicated(string no, int? orderId = null)
    //{
    //    no = no.Trim();
    //    var order = await orderDal.GetAsync(predicate: x => x.OrderNo == no, enableTracking: false);
    //    if (orderId == null && order != null) throw new BusinessException(Strings.DynamicExists, no);
    //    if (orderId != null && order != null && order.Id != orderId) throw new BusinessException(Strings.DynamicExists, no);
    //}

    //internal async Task OrderEmailCanNotBeDuplicated(string email, int? orderId = null)
    //{
    //    email = email.Trim();
    //    var order = await orderDal.GetAsync(predicate: x => x.Email == email, enableTracking: false);
    //    if (orderId == null && order != null) throw new BusinessException(Strings.DynamicExists, email);
    //    if (orderId != null && order != null && order.Id != orderId) throw new BusinessException(Strings.DynamicExists, email);
    //}

    //internal async Task OrderPhoneCanNotBeDuplicated(string phone, int? orderId = null)
    //{
    //    phone = phone.Trim();
    //    var order = await orderDal.GetAsync(predicate: x => x.Phone == phone, enableTracking: false);
    //    if (orderId == null && order != null) throw new BusinessException(Strings.DynamicExists, phone);
    //    if (orderId != null && order != null && order.Id != orderId) throw new BusinessException(Strings.DynamicExists, phone);
    //}

    //internal async Task OrdersShouldExistsAndActiveByIds(List<int> orderIds)
    //{
    //    var orders = !orderIds.Except(await orderDal.Query().AsNoTracking().Where(x => x.IsActive).Select(x => x.Id).ToListAsync()).Any();
    //    if (!orders) throw new BusinessException(Strings.DynamicNotFoundOrActive, Strings.Order);
    //}
}