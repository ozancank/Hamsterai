using Domain.Constants;
using Infrastructure.Payment.Models;
using OCK.Core.Exceptions.CustomExceptions;
using OCK.Core.Interfaces;
using OCK.Core.Utilities;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Payment.Sipay;

public sealed class SipayApi(IHttpClientFactory httpClientFactory) : IPaymentApi
{
    private static readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
    private static string _token = string.Empty;
    private static DateTime _expires = DateTime.MinValue;

    private async Task<HttpClient> CreateHttpClient()
    {
        var client = httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(SipayConfiguration.TimeoutSecond);
        await GetToken();
        client.DefaultRequestHeaders.Add("Authorization", _token);
        return client;
    }

    private async Task GetToken()
    {
        var methodName = nameof(GetToken);
        try
        {
            if (_token.IsNotEmpty() && _expires > DateTime.UtcNow.AddHours(3)) return;

            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(SipayConfiguration.TimeoutSecond);
            var url = $"{SipayConfiguration.ApiUrl}/api/token";

            var data = new TokenRequestDto(SipayConfiguration.AppKey, SipayConfiguration.AppSecret);
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<SipayResponseDto<TokenResponseDto>>(content, _options) ?? throw new ExternalApiException(Strings.DynamicNotNull, nameof(content));
                if (res.StatusCode != 100 || res.Data == null) throw new ExternalApiException(content.Trim("{", "}"));
                _token = $"Bearer {res.Data.Token}";
                _expires = DateTime.Parse(res.Data.ExpiresAt).AddMinutes(-2);
            }
            else
                throw new ExternalApiException((await response.Content.ReadAsStringAsync()).Trim("{", "}"));
        }
        catch (Exception ex)
        {
            _token = string.Empty;
            _expires = DateTime.MinValue;
            Console.WriteLine($"{methodName} - Error: {ex.Message}");
            throw new ExternalApiException(ex.Message);
        }
    }

    private static string GenerateHashKey(string data)
    {
        var iv = CryptographyTools.GenerateRandomString(16);
        var password = CryptographyTools.ComputeSha1Hash(SipayConfiguration.AppSecret);
        var salt = CryptographyTools.GenerateRandomString(4);
        var saltWithPassword = CryptographyTools.ComputeSha256Hash($"{password}{salt}");
        var key = saltWithPassword[..32];
        var encrypted = CryptographyTools.EncryptWithAes256(data, key, iv);
        var msgEncryptedBundle = $"{iv}:{salt}:{encrypted}";
        var hashKey = msgEncryptedBundle.Replace("/", "__");
        return hashKey;
    }

    public async Task<GetPaymentResponseModel> GetPayment(string invoiceId, double amount)
    {
        var methodName = nameof(GetPayment);
        try
        {
            using var client = await CreateHttpClient();
            var url = $"{SipayConfiguration.ApiUrl}/api/checkstatus";

            var formattedAmount = amount.ToString("0.00", CultureInfo.InvariantCulture);
            var hashKey = GenerateHashKey($"{formattedAmount}|{invoiceId}|{SipayConfiguration.MerchantKey}");
            var data = new GetPaymentRequestDto(SipayConfiguration.MerchantKey, invoiceId, true, hashKey);
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<GetPaymentResponseDto>(content, _options) ?? throw new ExternalApiException(Strings.DynamicNotNull, nameof(content));
                if (res.StatusCode != 100) throw new ExternalApiException(content.Trim("{", "}"));
                var result = new GetPaymentResponseModel
                {
                    StatusCode = res.StatusCode,
                    StatusDescription = res.StatusDescription,
                    TransactionStatus = res.TransactionStatus,
                    OrderId = res.OrderId,
                    TransactionId = res.TransactionId,
                    Message = res.Message,
                    Reason = res.Reason,
                    BankStatusCode = res.BankStatusCode,
                    BankStatusDescription = res.BankStatusDescription,
                    InvoiceId = res.InvoiceId,
                    TotalRefundedAmount = res.TotalRefundedAmount,
                    ProductPrice = res.ProductPrice,
                    TransactionAmount = res.TransactionAmount,
                    RecurringId = res.RecurringId,
                    RefNumber = res.RefNumber,
                    RecurringPlanCode = res.RecurringPlanCode,
                    NextActionDate = res.NextActionDate,
                    RecurringStatus = res.RecurringStatus,
                    MerchantCommission = res.MerchantCommission,
                    UserCommission = res.UserCommission,
                    SettlementDate = res.SettlementDate,
                    MdStatus = res.MdStatus,
                    Installment = res.Installment
                };
                return result;
            }
            else
                throw new ExternalApiException((await response.Content.ReadAsStringAsync()).Trim("{", "}"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{methodName} - Error: {ex.Message}");
            throw new ExternalApiException(ex.Message);
        }
    }

    public async Task<GetRecurringModel> GetRequrring(string planCode, int recurringNumber)
    {
        var methodName = nameof(GetRequrring);
        try
        {
            using var client = await CreateHttpClient();
            var url = $"{SipayConfiguration.ApiUrl}/api/recurringPlan/query";

            var data = new GetRequrringRequestDto(SipayConfiguration.MerchantKey, planCode, recurringNumber.ToString());
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<GetRequrringResponseDto>(content, _options) ?? throw new ExternalApiException(Strings.DynamicNotNull, nameof(content));
                if (res.StatusCode != 100) throw new ExternalApiException(content.Trim("{", "}"));
                var result = new GetRecurringModel
                {
                    StatusCode = res.StatusCode,
                    Message = res.Message,
                    RecurringId = res.RecurringId,
                    FirstAmount = res.FirstAmount,
                    RecurringAmount = res.RecurringAmount,
                    TotalAmount = res.TotalAmount,
                    PaymentNumber = res.PaymentNumber,
                    PaymentInterval = res.PaymentInterval,
                    PaymentCycle = res.PaymentCycle,
                    FirstOrderId = res.FirstOrderId,
                    MerchantId = res.MerchantId,
                    CardNo = res.CardNo,
                    NextActionDate = res.NextActionDate,
                    RecurringStatus = res.RecurringStatus,
                    TransactionDate = res.TransactionDate,
                    TransactionHistories = [.. res.TransactionHistories.Select(x => new GetRecurringTransactionModel
                    {
                        Id = x.Id,
                        SaleRecurringId = x.SaleRecurringId,
                        SaleId = x.SaleId,
                        MerchantId = x.MerchantId,
                        SaleRecurringPaymentScheduleId = x.SaleRecurringPaymentScheduleId,
                        Amount = x.Amount,
                        ActionDate = x.ActionDate,
                        Status = x.Status,
                        RecurringNumber = x.RecurringNumber,
                        Attempts = x.Attempts,
                        Remarks = x.Remarks
                    })]
                };

                return result;
            }
            else
                throw new ExternalApiException((await response.Content.ReadAsStringAsync()).Trim("{", "}"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{methodName} - Error: {ex.Message}");
            throw new ExternalApiException(ex.Message);
        }
    }

    public async Task<bool> UpdateRecurringRequest(UpdateRecurringRequestModel model)
    {
        var methodName = nameof(GetRequrring);
        try
        {
            using var client = await CreateHttpClient();
            var url = $"{SipayConfiguration.ApiUrl}/api/recurringPlan/update";

            var payload = new Dictionary<string, string>
            {
                { "merchant_key", SipayConfiguration.MerchantKey },
                { "plan_code", model.PlanCode ?? string.Empty },
                { "recurring_amount", model.RecurringAmount! },
            };
            if (model.RecurringActive.HasValue) payload.Add("recurring_status", model.RecurringActive.Value ? "ACTIVE" : "INACTIVE");
            if (model.RecurringPaymentNumber.IsNotEmpty()) payload.Add("recurring_payment_number", model.RecurringPaymentNumber!);

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(payload),
            };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<SipayResponseDto>(content, _options) ?? throw new ExternalApiException(Strings.DynamicNotNull, nameof(content));
                if (res.StatusCode != 100) throw new ExternalApiException(content.Trim("{", "}"));

                return true;
            }
            else
                throw new ExternalApiException((await response.Content.ReadAsStringAsync()).Trim("{", "}"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{methodName} - Error: {ex.Message}");
            throw new ExternalApiException(ex.Message);
        }
    }

    private record SipayResponseDto(
    [property: JsonPropertyName("status_code")] int StatusCode,
    [property: JsonPropertyName("status_description")] string StatusDescription
    ) : IDto;

    private sealed record SipayResponseDto<T>(
        int StatusCode,
        string StatusDescription,
        [property: JsonPropertyName("data")] T? Data
        ) : SipayResponseDto(StatusCode, StatusDescription);

    private sealed record TokenRequestDto(
        [property: JsonPropertyName("app_id")] string AppId,
        [property: JsonPropertyName("app_secret")] string AppSecret
        ) : IDto;

    private sealed record TokenResponseDto(
        [property: JsonPropertyName("token")] string Token,
        [property: JsonPropertyName("is_3d")] int Is3d,
        [property: JsonPropertyName("expires_at")] string ExpiresAt
        ) : IDto;

    private sealed record GetPaymentRequestDto(
        [property: JsonPropertyName("merchant_key")] string MerchantKey,
        [property: JsonPropertyName("invoice_id")] string InvoiceId,
        [property: JsonPropertyName("include_pending_status")] bool IncludePendingStatus,
        [property: JsonPropertyName("hash_key")] string HashKey
        ) : IDto;

    private sealed record GetPaymentResponseDto(
        [property: JsonPropertyName("status_code")] int StatusCode,
        [property: JsonPropertyName("status_description")] string StatusDescription,
        [property: JsonPropertyName("transaction_status")] string TransactionStatus,
        [property: JsonPropertyName("order_id")] string OrderId,
        [property: JsonPropertyName("transaction_id")] string TransactionId,
        [property: JsonPropertyName("message")] string Message,
        [property: JsonPropertyName("reason")] string Reason,
        [property: JsonPropertyName("bank_status_code")] string BankStatusCode,
        [property: JsonPropertyName("bank_status_description")] string BankStatusDescription,
        [property: JsonPropertyName("invoice_id")] string InvoiceId,
        [property: JsonPropertyName("total_refunded_amount")] int TotalRefundedAmount,
        [property: JsonPropertyName("product_price")] string ProductPrice,
        [property: JsonPropertyName("transaction_amount")] int TransactionAmount,
        [property: JsonPropertyName("recurring_id")] int RecurringId,
        [property: JsonPropertyName("ref_number")] string RefNumber,
        [property: JsonPropertyName("transaction_type")] string TransactionType,
        [property: JsonPropertyName("original_bank_error_code")] string OriginalBankErrorCode,
        [property: JsonPropertyName("original_bank_error_description")] string OriginalBankErrorDescription,
        [property: JsonPropertyName("cc_no")] string CcNo,
        [property: JsonPropertyName("payment_reason_code")] string PaymentReasonCode,
        [property: JsonPropertyName("payment_reason_code_detail")] string PaymentReasonCodeDetail,
        [property: JsonPropertyName("recurring_plan_code")] string RecurringPlanCode,
        [property: JsonPropertyName("next_action_date")] string NextActionDate,
        [property: JsonPropertyName("recurring_status")] string RecurringStatus,
        [property: JsonPropertyName("merchant_commission")] string MerchantCommission,
        [property: JsonPropertyName("user_commission")] string UserCommission,
        [property: JsonPropertyName("settlement_date")] string SettlementDate,
        [property: JsonPropertyName("md_status")] int MdStatus,
        [property: JsonPropertyName("installment")] int Installment
        ) : IDto;

    private sealed record GetRequrringRequestDto(
        [property: JsonPropertyName("merchant_key")] string MerchantKey,
        [property: JsonPropertyName("plan_code")] string PlanCode,
        [property: JsonPropertyName("recurring_number")] string RecurringNumber
        ) : IDto;

    public sealed record GetRequrringResponseDto(
        [property: JsonPropertyName("status_code")] int StatusCode,
        [property: JsonPropertyName("message")] string Message,
        [property: JsonPropertyName("recurring_id")] int RecurringId,
        [property: JsonPropertyName("plan_code")] string PlanCode,
        [property: JsonPropertyName("currency")] string Currency,
        [property: JsonPropertyName("currency_symbol")] string CurrencySymbol,
        [property: JsonPropertyName("first_amount")] double FirstAmount,
        [property: JsonPropertyName("recurring_amount")] double RecurringAmount,
        [property: JsonPropertyName("total_amount")] double TotalAmount,
        [property: JsonPropertyName("payment_number")] int PaymentNumber,
        [property: JsonPropertyName("payment_interval")] int PaymentInterval,
        [property: JsonPropertyName("payment_cycle")] string PaymentCycle,
        [property: JsonPropertyName("first_order_id")] string FirstOrderId,
        [property: JsonPropertyName("merchant_id")] int MerchantId,
        [property: JsonPropertyName("card_no")] string CardNo,
        [property: JsonPropertyName("next_action_date")] string NextActionDate,
        [property: JsonPropertyName("recurring_status")] string RecurringStatus,
        [property: JsonPropertyName("transaction_date")] string TransactionDate,
        [property: JsonPropertyName("transactionHistories")] IReadOnlyList<GetRequrringTransactionDto> TransactionHistories
        ) : IDto;

    public sealed record GetRequrringTransactionDto(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("sale_recurring_id")] int SaleRecurringId,
        [property: JsonPropertyName("sale_id")] int SaleId,
        [property: JsonPropertyName("merchant_id")] int MerchantId,
        [property: JsonPropertyName("sale_recurring_payment_schedule_id")] int SaleRecurringPaymentScheduleId,
        [property: JsonPropertyName("amount")] double Amount,
        [property: JsonPropertyName("action_date")] string ActionDate,
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("recurring_number")] int RecurringNumber,
        [property: JsonPropertyName("attempts")] int Attempts,
        [property: JsonPropertyName("remarks")] string Remarks
        ) : IDto;
}