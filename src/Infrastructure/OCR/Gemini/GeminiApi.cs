using Domain.Constants;
using Infrastructure.OCR.Gemini.Models;
using Infrastructure.OCR.Models;
using OCK.Core.Exceptions.CustomExceptions;
using OCK.Core.Logging.Serilog;
using System.Text;
using System.Text.Json;

namespace Infrastructure.OCR.Gemini;

public class GeminiApi(IHttpClientFactory httpClientFactory, LoggerServiceBase loggerServiceBase) : IOcrApi
{
    private static readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
    private readonly int _apiTimeoutSecond = 20;

    public async Task<OcrResponseModel> GetTextFromImage(OcrRequestModel model)
    {
        var baseUrl = AppOptions.OCR_Url;
        var result = new OcrResponseModel();
        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(_apiTimeoutSecond);

            var data = new GeminiOcrRequestModel
            {
                FileName = model.FileName
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/image-ocr")
            {
                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var responseJson = JsonSerializer.Deserialize<GeminiOcrResponseModel>(content, _options);

            var isClassic = responseJson.Message.Contains("##classic##", StringComparison.OrdinalIgnoreCase);

            var lines = responseJson.Message.Trim('\r').Split("\n").ToList();

            if (lines.Contains("Görselde metin veya yazı yok", StringComparer.OrdinalIgnoreCase)
                || lines.Contains("Görselde metin yok", StringComparer.OrdinalIgnoreCase)
                || lines.Contains("Görselde yazı yok", StringComparer.OrdinalIgnoreCase))
                throw new ExternalApiException("Görselde metin veya yazı yok.");

            if (!isClassic)
            {
                var options = new bool[5];
                options[0] = lines.Any(x => x.Trim().StartsWith("A) ", StringComparison.CurrentCultureIgnoreCase));
                options[1] = lines.Any(x => x.Trim().StartsWith("B) ", StringComparison.CurrentCultureIgnoreCase));
                options[2] = lines.Any(x => x.Trim().StartsWith("C) ", StringComparison.CurrentCultureIgnoreCase));
                options[3] = lines.Any(x => x.Trim().StartsWith("D) ", StringComparison.CurrentCultureIgnoreCase));
                options[4] = lines.Any(x => x.Trim().StartsWith("E) ", StringComparison.CurrentCultureIgnoreCase));

                var optionCount = options.Count(x => x);
                if (!optionCount.Between(3, 5)) throw new ExternalApiException($"OCR'dan dönen şıklar 3 ile 5 arasında olmalıdır. Seçenek sayısı: {optionCount}");

                int indexA = lines.FindIndex(x => x.StartsWith("A) "));

                if (indexA > 0)
                {
                    if (indexA == 1 || lines[indexA - 1].IsNotEmpty())
                        lines.Insert(indexA, string.Empty);
                    else if (indexA > 1 && lines[indexA - 2].IsEmpty())
                        lines.RemoveAt(indexA - 1);
                }

                for (int i = indexA + 1; i < lines.Count; i++)
                {
                    if (lines[i].StartsWith("B) ") || lines[i].StartsWith("C) ") || lines[i].StartsWith("D) ") || lines[i].StartsWith("E) "))
                    {
                        if (lines[i - 1].IsEmpty())
                        {
                            lines.RemoveAt(i - 1);
                            i--;
                        }
                    }
                }
            }
            result.Text = string.Join("\n", lines);

            if (result.Text.Contains("argument should contain only ASCII characters")) throw new ExternalApiException("OCR ASCII hatası verdi.");
        }
        catch (Exception ex)
        {
            loggerServiceBase.Error($"GetTextFromImage {ex?.InnerException?.Message}*{ex?.Message}");
        }
        return result;
    }
}