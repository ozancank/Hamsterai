using OCK.Core.Interfaces;
using System.Text.Json.Serialization;

namespace Infrastructure.Payment.Sipay.Models;

public sealed class SipayRecurringWebHookRequestModel : IRequestModel
{
    [JsonPropertyName("merchant_key")]
    public string? MerchantKey { get; set; }

    [JsonPropertyName("invoice_id")]
    public string? InvoiceId { get; set; }

    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    [JsonPropertyName("product_price")]
    public string? ProductPrice { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("recurring_number")]
    public string? RecurringNumber { get; set; }

    [JsonPropertyName("plan_code")]
    public string? PlanCode { get; set; }

    [JsonPropertyName("action_date")]
    public string? ActionDate { get; set; }
}