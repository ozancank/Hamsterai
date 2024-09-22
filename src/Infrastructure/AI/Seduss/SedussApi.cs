using Domain.Constants;
using Infrastructure.AI.Seduss.Models;
using Infrastructure.Constants;
using System.Text;
using System.Text.Json;

namespace Infrastructure.AI.Seduss;

public class SedussApi(IHttpClientFactory httpClientFactory) : IQuestionApi
{
    private static readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

    public async Task<QuestionTOResponseModel> AskQuestionOcr(string base64, Guid id, string lessonName)
    {
        QuestionTOResponseModel question = null;
        try
        {
            using var client = httpClientFactory.CreateClient();
            var busy = false;

            var request = new HttpRequestMessage(HttpMethod.Post, $"{AppOptions.AIApiLite}/Model_Available");
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            busy = Convert.ToBoolean(content);

            if (busy)
            {
                //question = new QuestionTOResponseModel { Soru_OCR = base64 };
                //await InfrastructureDelegates.UpdateQuestionOcr.Invoke(question, id, QuestionStatus.SendAgain);
                //return question;
            }

            request = new HttpRequestMessage(HttpMethod.Post, $"{AppOptions.AIApiLite}/Soru_TO")
            {
                Content = new StringContent(JsonSerializer.Serialize(new { content = base64 }), Encoding.UTF8, "application/json")
            };

            response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            content = await response.Content.ReadAsStringAsync();
            question = JsonSerializer.Deserialize<QuestionTOResponseModel>(content, _options);

            if (InfrastructureDelegates.UpdateQuestionOcr != null)
                await InfrastructureDelegates.UpdateQuestionOcr.Invoke(question, id, QuestionStatus.Answered);

            return question;
        }
        catch
        {
            if (InfrastructureDelegates.UpdateQuestionOcr != null)
                await InfrastructureDelegates.UpdateQuestionOcr.Invoke(question, id, QuestionStatus.Error);
            throw;
        }
        finally
        {
            // await EndChat();
        }
    }

    public async Task<QuestionITOResponseModel> AskQuestionOcrImage(string base64, Guid id, string lessonName)
    {
        QuestionITOResponseModel question = null;
        try
        {
            using var client = httpClientFactory.CreateClient();
            var busy = false;

            var request = new HttpRequestMessage(HttpMethod.Post, $"{AppOptions.AIApiLite}/Model_Available");
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            busy = Convert.ToBoolean(content);

            if (busy)
            {
                //question = new QuestionITOResponseModel { Soru_OCR = base64 };
                //await InfrastructureDelegates.UpdateQuestionOcrImage.Invoke(question, id, QuestionStatus.SendAgain);
                //return question;
            }

            request = new HttpRequestMessage(HttpMethod.Post, $"{AppOptions.AIApiLite}/Soru_ITO")
            {
                Content = new StringContent(JsonSerializer.Serialize(new { content = base64 }), Encoding.UTF8, "application/json")
            };

            response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            content = await response.Content.ReadAsStringAsync();
            question = JsonSerializer.Deserialize<QuestionITOResponseModel>(content, _options);

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
            // await EndChat();
        }
    }

    public async Task<SimilarResponseModel> GetSimilarQuestion(string base64, Guid id, string lessonName)
    {
        SimilarResponseModel similar = null;
        try
        {
            using var client = httpClientFactory.CreateClient();

            var busy = false;

            var request = new HttpRequestMessage(HttpMethod.Post, $"{AppOptions.AIApiLite}/Model_Available");
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            busy = Convert.ToBoolean(content);

            if (busy)
            {
                //similar = new SimilarResponseModel { Soru_OCR = base64 };
                //await InfrastructureDelegates.UpdateSimilarQuestionAnswer.Invoke(similar, id, QuestionStatus.SendAgain);
                //return similar;
            }

            request = new HttpRequestMessage(HttpMethod.Post, $"{AppOptions.AIApiLite}/Benzer")
            {
                Content = new StringContent(JsonSerializer.Serialize(new { content = base64 }), Encoding.UTF8, "application/json")
            };

            response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            content = await response.Content.ReadAsStringAsync();
            similar = JsonSerializer.Deserialize<SimilarResponseModel>(content, _options);

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
            //await EndChat();
        }
    }

    //private async Task EndChat()
    //{
    //    var client = httpClientFactory.CreateClient();
    //    var response = await client.PostAsync($"{AppOptions.AIApiLite}/end-chat", null);
    //    response.EnsureSuccessStatusCode();
    //}
}