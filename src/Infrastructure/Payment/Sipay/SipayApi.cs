using Domain.Constants;
using Infrastructure.Payment.Configuration;
using OCK.Core.Exceptions.CustomExceptions;
using OCK.Core.Interfaces;
using OCK.Core.Utilities;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Payment.Sipay;

public class SipayApi(IHttpClientFactory httpClientFactory) : IPaymentApi
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

    public async Task<bool> PaymentControl(string invoiceId, double amount)
    {
        var methodName = nameof(PaymentControl);
        try
        {
            using var client = await CreateHttpClient();
            var url = $"{SipayConfiguration.ApiUrl}/api/checkstatus";

            var formattedAmount = amount.ToString("0.00", CultureInfo.InvariantCulture);
            var hashKey = GenerateHashKey($"{formattedAmount}|{invoiceId}|{SipayConfiguration.MerchantKey}");
            var data = new CheckStatusRequestDto(SipayConfiguration.MerchantKey, invoiceId, true, hashKey);
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<CheckStatusResponseDto>(content, _options) ?? throw new ExternalApiException(Strings.DynamicNotNull, nameof(content));
                if (res.StatusCode != 100) throw new ExternalApiException(content.Trim("{", "}"));
            }
            else
                throw new ExternalApiException((await response.Content.ReadAsStringAsync()).Trim("{", "}"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{methodName} - Error: {ex.Message}");
            throw new ExternalApiException(ex.Message);
        }
        return false;
    }

    private record SipayResponseDto<T>(
        [property: JsonPropertyName("status_code")] int StatusCode,
        [property: JsonPropertyName("status_description")] string StatusDescription,
        [property: JsonPropertyName("data")] T? Data
        ) : IDto;
    private record TokenRequestDto(
        [property: JsonPropertyName("app_id")] string AppId,
        [property: JsonPropertyName("app_secret")] string AppSecret
        ) : IDto;
    private record TokenResponseDto(
        [property: JsonPropertyName("token")] string Token,
        [property: JsonPropertyName("is_3d")] int Is3d,
        [property: JsonPropertyName("expires_at")] string ExpiresAt
        ) : IDto;
    private record CheckStatusRequestDto(
        [property: JsonPropertyName("merchant_key")] string MerchantKey,
        [property: JsonPropertyName("invoice_id")] string InvoiceId,
        [property: JsonPropertyName("include_pending_status")] bool IncludePendingStatus,
        [property: JsonPropertyName("hash_key")] string HashKey
        ) : IDto;

    private record CheckStatusResponseDto(
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
}