using Domain.Constants;
using Infrastructure.AI.Seduss.Dtos;
using Infrastructure.AI.Seduss.Models;
using Infrastructure.Constants;
using OCK.Core.Exceptions.CustomExceptions;
using System.Text;
using System.Text.Json;

namespace Infrastructure.AI.Seduss;

public class SedussApi(IHttpClientFactory httpClientFactory) : IQuestionApi
{
    private static readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
    private const double _apiTimeoutMinute = 10;
    private static readonly string[] _answersOptions = ["A", "B", "C", "D", "E"];

    private static string BaseUrl(long userId)
    {
        return userId switch
        {
            18 => AppOptions.AI_GPT4o,
            19 => AppOptions.AI_8B_GPT4o,
            20 => AppOptions.AI_8B,
            _ => AppOptions.AI_Baz
        };
    }

    public async Task<QuestionTOResponseModel> AskQuestionOcr(QuestionApiModel model)
    {
        var baseUrl = BaseUrl(model.UserId);
        QuestionTOResponseModel answer = null;
        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(_apiTimeoutMinute);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/Model_Available");
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

            request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/Soru_TO")
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
        catch (Exception ex)
        {
            if (InfrastructureDelegates.UpdateQuestionOcr != null)
                await InfrastructureDelegates.UpdateQuestionOcr.Invoke(answer, new(model.Id, QuestionStatus.Error, model.UserId));
            throw (ExternalApiException)ex;
        }
        finally
        {
            await EndChat(baseUrl);
        }
    }

    public async Task<QuestionITOResponseModel> AskQuestionOcrImage(QuestionApiModel model)
    {
        var baseUrl = BaseUrl(model.UserId);
        QuestionITOResponseModel answer = null;
        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(_apiTimeoutMinute);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/Model_Available");
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

            request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/Soru_ITO")
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
        catch (Exception ex)
        {
            if (InfrastructureDelegates.UpdateQuestionOcrImage != null)
                await InfrastructureDelegates.UpdateQuestionOcrImage.Invoke(answer, new(model.Id, QuestionStatus.Error, model.UserId));
            throw (ExternalApiException)ex;
        }
        finally
        {
            await EndChat(baseUrl);
        }
    }

    public async Task<SimilarResponseModel> GetSimilarQuestion(QuestionApiModel model)
    {
        var baseUrl = BaseUrl(model.UserId);
        SimilarResponseModel similar = null;
        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(_apiTimeoutMinute);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/Model_Available");
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

            request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/Benzer")
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
        catch (Exception ex)
        {
            if (InfrastructureDelegates.UpdateSimilarQuestionAnswer != null)
                await InfrastructureDelegates.UpdateSimilarQuestionAnswer.Invoke(similar, new(model.Id, QuestionStatus.Error, model.UserId));
            throw (ExternalApiException)ex;
        }
        finally
        {
            await EndChat(baseUrl);
        }
    }

    public async Task<QuizResponseModel> GetQuizQuestions(QuizApiModel model)
    {
        var baseUrl = BaseUrl(model.UserId);
        var similars = new QuizResponseModel
        {
            Questions = []
        };

        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(_apiTimeoutMinute);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/Model_Available");
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

            request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/Multi_Benzer")
            {
                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
            };

            response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            content = await response.Content.ReadAsStringAsync();
            similars = JsonSerializer.Deserialize<QuizResponseModel>(content, _options);
            similars.Questions.ForEach(x =>
            {
                x.RightOption = x.RightOption.IsNotEmpty()
                    ? x.RightOption.Trim("Cevap", ":", ")", "-").ToUpper()
                    : throw new ExternalApiException(Strings.DynamicNotEmpty.Format(Strings.RightOption));

                if (!_answersOptions.Contains(x.RightOption, StringComparer.OrdinalIgnoreCase))
                    throw new ExternalApiException(Strings.DynamicBetween.Format(Strings.RightOption, "A", "E"));
            });
        }
        catch (Exception ex)
        {
            throw (ExternalApiException)ex;
        }
        finally
        {
            await EndChat(baseUrl);
        }

        return similars;
    }

    private async Task EndChat(string baseUrl)
    {
        var client = httpClientFactory.CreateClient();
        var response = await client.PostAsync($"{baseUrl}/end-chat", null);
        response.EnsureSuccessStatusCode();
    }
}