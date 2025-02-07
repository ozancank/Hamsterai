using OCK.Core.Interfaces;

namespace Infrastructure.Payment.Models;

public sealed class UpdateRecurringRequestModel : IResponseModel
{
    public string? PlanCode { get; set; }
    public string? RecurringAmount { get; set; }
    public bool? RecurringActive { get; set; }
    public string? RecurringPaymentNumber { get; set; }
}