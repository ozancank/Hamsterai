using OCK.Core.Interfaces;

namespace Infrastructure.Payment;

public interface IPaymentApi : IExternalApi
{
    Task<bool> PaymentControl(string invoiceId, double amount);

}