using Domain.Constants;
using OCK.Core.Exceptions.CustomExceptions;
using OCK.Core.Utilities;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Notification.OneSignal;

public sealed class OneSignalApi(IHttpClientFactory httpClientFactory) : INotificationApi
{
    private static bool? _isNullable = null;
    private static bool? _isString = null;

    public async Task<bool> PushNotification<T>(NotificationModel<T> notificationModel)
    {
        ArgumentNullException.ThrowIfNull(notificationModel);

        _isNullable ??= ReflectionTools.IsNullableType(typeof(T));
        _isString ??= typeof(T) == typeof(string);

        if (!_isString.Value) throw new BusinessException(Strings.InvalidValue);

        if (_isNullable.Value && notificationModel.List == null)
            throw new BusinessException(Strings.InvalidValue);

        var tokens = notificationModel.List?.Cast<string>().ToList();

        if (tokens?.Count == 0) throw new BusinessException(Strings.DynamicNotEmpty, Strings.Token);

        var body = new Body(
            AppId: OneSignalConfiguration.AppId,
            Title: new(notificationModel.Title ?? string.Empty, notificationModel.Title ?? string.Empty),
            Message: new(notificationModel.Body ?? string.Empty, notificationModel.Body ?? string.Empty),
            Datas: notificationModel.Datas,
            Aliases: new(tokens!),
            TargetChannel: "push",
            TemplateId: OneSignalConfiguration.TemplateId,
            UniqueId: Guid.NewGuid().ToString()
            );

        var response = await PushNotification(body);

        return response.IsNotEmpty();
    }

    private HttpClient CreateClient()
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(OneSignalConfiguration.BaseUrl);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", $"Key {OneSignalConfiguration.SecretKey}");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client;
    }

    private async Task<string> PushNotification(Body body)
    {
        using var client = CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "/notifications")
        {
            Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
        };
        var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        var result = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode) return string.Empty;
        return result;
    }

    private record Body(
        [property: JsonPropertyName("app_id")] string AppId,
        [property: JsonPropertyName("headings")] Headings Title,
        [property: JsonPropertyName("contents")] Contents Message,
        [property: JsonPropertyName("data")] IReadOnlyDictionary<string, string> Datas,
        [property: JsonPropertyName("include_aliases")] IncludeAliases Aliases,
        [property: JsonPropertyName("target_channel")] string TargetChannel,
        [property: JsonPropertyName("template_id")] string TemplateId,
        [property: JsonPropertyName("idempotency_key")] string UniqueId);

    private record Contents(
        [property: JsonPropertyName("en")] string En,
        [property: JsonPropertyName("tr")] string Tr);

    private record Headings(
        [property: JsonPropertyName("en")] string En,
        [property: JsonPropertyName("tr")] string Tr);

    private record IncludeAliases(
        [property: JsonPropertyName("external_id")] List<string> UserIds);
}