using Infrastructure.Payment.Models;
using OCK.Core.Interfaces;

namespace Infrastructure.Payment;

public interface IPaymentApi : IExternalApi
{
    Task<GetPaymentResponseModel> GetPayment(string invoiceId, double amount);

    Task<GetRecurringModel> GetRequrring(string planCode, int recurringNumber);

    Task<bool> UpdateRecurringRequest(UpdateRecurringRequestModel model);
}