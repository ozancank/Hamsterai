using System.Reflection;

namespace Application.Features.Orders.Constants;

public sealed class OrderStatics : IStatic
{
    public static PropertyInfo[] AddPaymentSipayModelForAddOrderModelStringProperties { get; internal set; } = [];
}