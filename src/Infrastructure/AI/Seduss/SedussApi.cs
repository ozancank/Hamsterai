using Domain.Constants;
using Infrastructure.AI.Seduss.Models;
using Infrastructure.Constants;
using OCK.Core.Exceptions.CustomExceptions;
using System.Text;
using System.Text.Json;

namespace Infrastructure.AI.Seduss;

public class SedussApi(IHttpClientFactory httpClientFactory) : IQuestionApi
{
    private static readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

    public async Task<string> AskQuestion(string base64, Guid id)
    {
        var question = string.Empty;
        try
        {
            using var client = httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, $"{AppOptions.AIApi}/ocr-text")
            {
                Content = new StringContent(JsonSerializer.Serialize(new { content = base64 }), Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var ocr = JsonSerializer.Deserialize<OcrModel>(content, _options);
                question = ocr.Text;
            }

            request = new HttpRequestMessage(HttpMethod.Post, $"{AppOptions.AIApi}/image")
            {
                Content = new StringContent(JsonSerializer.Serialize(new { content = base64 }), Encoding.UTF8, "application/json")
            };

            response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            using var reader = await response.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(reader, Encoding.UTF8);

            var builder = new StringBuilder();
            var buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = await reader.ReadAsync(buffer)) > 0)
            {
                var token = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                if (token.Contains("?!false?!"))
                    throw new ExternalApiException(Strings.ExternalApiError);

                if (token.EndsWith('.') || token.EndsWith('!') || token.EndsWith('?'))
                    builder.AppendLine(token);
                else
                    builder.Append(token);
            }

            var answer = builder.ToString();
            if (InfrastructureDelegates.UpdateQuestion != null)
                await InfrastructureDelegates.UpdateQuestion.Invoke(answer, question, id, QuestionStatus.Answered);

            return answer;
        }
        catch
        {
            if (InfrastructureDelegates.UpdateQuestion != null)
                await InfrastructureDelegates.UpdateQuestion.Invoke(string.Empty, question, id, QuestionStatus.Error);
            throw;
        }
        finally
        {
            await EndChat();
        }
    }

    public async Task<QuestionOcrModel> AskQuestionOcr(string base64, Guid id)
    {
        QuestionOcrModel question = null;
        try
        {
            using var client = httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, $"{AppOptions.AIApi}/Soru_TO")
            {
                Content = new StringContent(JsonSerializer.Serialize(new { content = base64 }), Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            question = JsonSerializer.Deserialize<QuestionOcrModel>(content, _options);

            if (InfrastructureDelegates.UpdateQuestionOcr != null)
                await InfrastructureDelegates.UpdateQuestionOcr.Invoke(question, id, QuestionStatus.Answered);

            return question;
        }
        catch
        {
            if (InfrastructureDelegates.UpdateQuestionOcr != null)
                await InfrastructureDelegates.UpdateQuestionOcr.Invoke(question, id, QuestionStatus.Answered);
            throw;
        }
        finally
        {
            await EndChat();
        }
    }

    public async Task<QuestionOcrImageModel> AskQuestionOcrImage(string base64, Guid id)
    {
        QuestionOcrImageModel question = null;
        try
        {
            using var client = httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, $"{AppOptions.AIApi}/Soru_ITO")
            {
                Content = new StringContent(JsonSerializer.Serialize(new { content = base64 }), Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            question = JsonSerializer.Deserialize<QuestionOcrImageModel>(content, _options);

            if (InfrastructureDelegates.UpdateQuestionOcrImage != null)
                await InfrastructureDelegates.UpdateQuestionOcrImage.Invoke(question, id, QuestionStatus.Answered);

            return question;
        }
        catch
        {
            if (InfrastructureDelegates.UpdateQuestionOcrImage != null)
                await InfrastructureDelegates.UpdateQuestionOcrImage.Invoke(question, id, QuestionStatus.Answered);
            throw;
        }
        finally
        {
            await EndChat();
        }
    }

    public async Task<SimilarModel> GetSimilarQuestion(string base64, Guid id)
    {
        SimilarModel similar = null;
        try
        {
            using var client = httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, $"{AppOptions.AIApi}/Benzer")
            {
                Content = new StringContent(JsonSerializer.Serialize(new { content = base64 }), Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            similar = JsonSerializer.Deserialize<SimilarModel>(content, _options);

            if (InfrastructureDelegates.UpdateSimilarQuestionAnswer != null)
                await InfrastructureDelegates.UpdateSimilarQuestionAnswer.Invoke(similar, id, QuestionStatus.Answered);

            return similar;
        }
        catch
        {
            if (InfrastructureDelegates.UpdateSimilarQuestionAnswer != null)
                await InfrastructureDelegates.UpdateSimilarQuestionAnswer.Invoke(similar, id, QuestionStatus.Error);
            throw;
        }
        finally
        {
            await EndChat();
        }
    }

    private async Task EndChat()
    {
        var client = httpClientFactory.CreateClient();
        var response = await client.PostAsync($"{AppOptions.AIApi}/end-chat", null);
        response.EnsureSuccessStatusCode();
    }
}