using Domain.Constants;
using Infrastructure.AI.Seduss.Dtos;
using Infrastructure.AI.Seduss.Models;
using Infrastructure.Constants;
using System.Text;
using System.Text.Json;

namespace Infrastructure.AI.Seduss;

public class SedussApi(IHttpClientFactory httpClientFactory) : IQuestionApi
{
    private static readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
    private const double _apiTimeoutMinute = 10;

    public async Task<QuestionTOResponseModel> AskQuestionOcr(QuestionApiModel model)
    {
        QuestionTOResponseModel answer = null;
        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(_apiTimeoutMinute);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{AppOptions.AIApiLite}/Model_Available");
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var available = Convert.ToBoolean(content);

            if (!available)
            {
                answer = new QuestionTOResponseModel { QuestionText = model.Base64 };
                await InfrastructureDelegates.UpdateQuestionOcr.Invoke(answer, new(model.Id, QuestionStatus.SendAgain, model.UserId));
                return answer;
            }

            var data = new QuestionRequestModel
            {
                QuestionImage = model.Base64,
                LessonName = model.LessonName?.Trim().ToLower() ?? string.Empty
            };

            request = new HttpRequestMessage(HttpMethod.Post, $"{AppOptions.AIApiOriginal}/Soru_TO")
            {
                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
            };

            response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            content = await response.Content.ReadAsStringAsync();
            answer = JsonSerializer.Deserialize<QuestionTOResponseModel>(content, _options);

            answer.RightOption = answer.RightOption.Trim("Cevap", ":", ")", "-").ToUpper();

            if (InfrastructureDelegates.UpdateQuestionOcr != null)

                await InfrastructureDelegates.UpdateQuestionOcr.Invoke(answer, new(model.Id, QuestionStatus.Answered, model.UserId));

            return answer;
        }
        catch
        {
            if (InfrastructureDelegates.UpdateQuestionOcr != null)
                await InfrastructureDelegates.UpdateQuestionOcr.Invoke(answer, new(model.Id, QuestionStatus.Error, model.UserId));
            throw;
        }
        finally
        {
            await EndChat();
        }
    }

    public async Task<QuestionITOResponseModel> AskQuestionOcrImage(QuestionApiModel model)
    {
        QuestionITOResponseModel answer = null;
        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(_apiTimeoutMinute);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{AppOptions.AIApiOriginal}/Model_Available");
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var available = Convert.ToBoolean(content);

            if (!available)
            {
                answer = new QuestionITOResponseModel { QuestionText = model.Base64 };
                await InfrastructureDelegates.UpdateQuestionOcrImage.Invoke(answer, new(model.Id, QuestionStatus.SendAgain, model.UserId));
                return answer;
            }

            var data = new QuestionRequestModel
            {
                QuestionImage = model.Base64,
                LessonName = model.LessonName?.Trim().ToLower() ?? string.Empty
            };

            request = new HttpRequestMessage(HttpMethod.Post, $"{AppOptions.AIApiOriginal}/Soru_ITO")
            {
                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"),
            };

            response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            content = await response.Content.ReadAsStringAsync();
            answer = JsonSerializer.Deserialize<QuestionITOResponseModel>(content, _options);
            answer.RightOption = answer.RightOption.Trim("Cevap", ":", ")", "-").ToUpper();

            if (InfrastructureDelegates.UpdateQuestionOcrImage != null)
                await InfrastructureDelegates.UpdateQuestionOcrImage.Invoke(answer, new(model.Id, QuestionStatus.Answered, model.UserId));

            return answer;
        }
        catch
        {
            if (InfrastructureDelegates.UpdateQuestionOcrImage != null)
                await InfrastructureDelegates.UpdateQuestionOcrImage.Invoke(answer, new(model.Id, QuestionStatus.Error, model.UserId));
            throw;
        }
        finally
        {
            await EndChat();
        }
    }

    public async Task<SimilarResponseModel> GetSimilarQuestion(QuestionApiModel model)
    {
        SimilarResponseModel similar = null;
        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(_apiTimeoutMinute);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{AppOptions.AIApiLite}/Model_Available");
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var available = Convert.ToBoolean(content);

            if (!available)
            {
                similar = new SimilarResponseModel { QuestionText = model.Base64 };
                await InfrastructureDelegates.UpdateSimilarQuestionAnswer.Invoke(similar, new UpdateQuestionDto(model.Id, QuestionStatus.SendAgain, model.UserId));
                return similar;
            }

            var data = new QuestionRequestModel
            {
                QuestionImage = model.Base64,
                LessonName = model.LessonName?.Trim().ToLower() ?? string.Empty
            };

            request = new HttpRequestMessage(HttpMethod.Post, $"{AppOptions.AIApiLite}/Benzer")
            {
                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
            };

            response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            content = await response.Content.ReadAsStringAsync();
            similar = JsonSerializer.Deserialize<SimilarResponseModel>(content, _options);
            similar.RightOption = similar.RightOption.Trim("Cevap", ":", ")", "-").ToUpper();

            if (InfrastructureDelegates.UpdateSimilarQuestionAnswer != null)
                await InfrastructureDelegates.UpdateSimilarQuestionAnswer.Invoke(similar, new(model.Id, QuestionStatus.Answered, model.UserId));

            return similar;
        }
        catch
        {
            if (InfrastructureDelegates.UpdateSimilarQuestionAnswer != null)
                await InfrastructureDelegates.UpdateSimilarQuestionAnswer.Invoke(similar, new(model.Id, QuestionStatus.Error, model.UserId));
            throw;
        }
        finally
        {
            await EndChat();
        }
    }

    public async Task<QuizResponseModel> GetQuizQuestions(QuizApiModel model)
    {
        var similars = new QuizResponseModel
        {
            Questions = []
        };

        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(_apiTimeoutMinute);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{AppOptions.AIApiLite}/Model_Available");
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var available = Convert.ToBoolean(content);

            if (!available) return similars;

            var data = new QuizRequestModel
            {
                QuestionImages = model.QuestionImages,
                LessonName = model.LessonName?.Trim().ToLower() ?? string.Empty
            };

            request = new HttpRequestMessage(HttpMethod.Post, $"{AppOptions.AIApiLite}/Multi_Benzer")
            {
                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
            };

            response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            content = await response.Content.ReadAsStringAsync();
            similars = JsonSerializer.Deserialize<QuizResponseModel>(content, _options);
            similars.Questions.ForEach(x => x.RightOption = x.RightOption.IsNotEmpty() ? x.RightOption.Trim("Cevap", ":", ")", "-").ToUpper() : throw new Exception(Strings.DynamicNotEmpty.Format(Strings.RightOption)));
        }
        finally
        {
            await EndChat();
        }

        return similars;
    }

    private async Task EndChat()
    {
        var client = httpClientFactory.CreateClient();
        var response = await client.PostAsync($"{AppOptions.AIApiLite}/end-chat", null);
        response.EnsureSuccessStatusCode();
    }
}