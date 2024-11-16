using Domain.Constants;
using Infrastructure.AI.Seduss.Models;
using Infrastructure.Constants;
using OCK.Core.Exceptions.CustomExceptions;
using OCK.Core.Logging.Serilog;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace Infrastructure.AI.Seduss;

public sealed class SedussApi(IHttpClientFactory httpClientFactory, LoggerServiceBase loggerServiceBase) : IQuestionApi
{
    private static readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
    private const double _apiTimeoutSecond = 60;
    private static readonly string[] _answersOptions = ["A", "B", "C", "D", "E"];
    private const byte _tryAgainCount = 3;

    private static readonly string[] _emptyQuestion =
    [
        "Soruyu lütfen iletin.",
        "ancak asıl soru verilmemiş",
        "Soruyu lütfen bana verin",
        "Örnek sorunuzda yoktu",
        "asıl soruyu belirtin",
        "Lütfen soruyu yazın",
        "Lütfen soruyu paylaşın",
        "bana asıl soruyu vermen"
    ];

    private static string BaseUrl(string? url)
    {
        if (url.IsEmpty()) return AppOptions.AI_Default!;
        return url!;
    }

    private static void LogError(LoggerServiceBase logger, Exception ex, long userId, [CallerMemberName] string methodName = "")
    {
        logger.Error($"{methodName} - {DateTime.Now:yyyy-MM-dd HH:mm:ss}*{userId}*{ex?.InnerException?.Message}*{ex?.Message}");
    }

    private static QuestionStatus GetErrorQuestionStatus(Exception ex)
    {
        var status = ex.Message switch
        {
            var _ when ex.Message.Contains("Soru algılanamadı. Lütfen tekrar deneyiniz.", StringComparison.OrdinalIgnoreCase) => QuestionStatus.OcrError,
            var _ when ex.Message.Contains("HttpClient.Timeout", StringComparison.OrdinalIgnoreCase) => QuestionStatus.Timeout,
            _ => QuestionStatus.Error
        };
        if (ex is HttpRequestException) status = QuestionStatus.ConnectionError;
        return status;
    }

    public async Task<QuestionITOResponseModel> AskQuestionOcrImage(QuestionApiModel model)
    {
        var baseUrl = BaseUrl(model.AIUrl);
        var answer = new QuestionITOResponseModel();
        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(_apiTimeoutSecond);

            var url = $"{baseUrl}/question/sor";
            if (baseUrl.Contains("185.195.255.124")) url = $"{baseUrl}/Soru_ITO";

            var data = new QuestionRequestModel
            {
                QuestionImage = model.Base64,
                LessonName = model.LessonName?.Trim().ToLower().ReplaceTurkishToLatin().Replace(" ","_") ?? string.Empty
            };
            Console.WriteLine(data.LessonName);

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                answer = JsonSerializer.Deserialize<QuestionITOResponseModel>(content, _options) ?? throw new ExternalApiException(Strings.DynamicNotNull, nameof(answer));

                if (answer.QuestionText.EmptyOrTrim().Contains("Soru algılanamadı. Lütfen tekrar deneyiniz.", StringComparison.OrdinalIgnoreCase))
                {
                    await InfrastructureDelegates.UpdateQuestionOcrImage?.Invoke(answer, new(model.Id, QuestionStatus.OcrError, model.UserId, baseUrl))!;
                    return answer;
                }

                if (!model.ExcludeQuiz)
                {
                    answer.RightOption = answer.RightOption.IsNotEmpty()
                        ? answer.RightOption.Trim("Cevap", ".", ":", ")", "-").ToUpper()[..1]
                        : throw new ExternalApiException(Strings.DynamicNotEmpty.Format(Strings.RightOption));

                    if (!_answersOptions.Contains(answer.RightOption, StringComparer.OrdinalIgnoreCase))
                        throw new ExternalApiException(Strings.DynamicBetween.Format(Strings.RightOption, "A", "E"));
                }

                answer.AnswerText = answer.AnswerText.EmptyOrTrim().Replace("Cevap X", string.Empty, StringComparison.OrdinalIgnoreCase);

                if (InfrastructureDelegates.UpdateQuestionOcrImage != null)
                    await InfrastructureDelegates.UpdateQuestionOcrImage.Invoke(answer, new(model.Id, QuestionStatus.Answered, model.UserId, baseUrl));
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new ExternalApiException(content.Trim("{", "}"));
            }
        }
        catch (Exception ex)
        {
            var status = GetErrorQuestionStatus(ex);
            await InfrastructureDelegates.UpdateQuestionOcrImage?.Invoke(answer, new(model.Id, status, model.UserId, baseUrl, ex.Message))!;
            LogError(loggerServiceBase, ex, model.UserId);
        }

        return answer;
    }

    public async Task<SimilarResponseModel> GetSimilar(QuestionApiModel model)
    {
        var baseUrl = BaseUrl(model.AIUrl);
        var similar = new SimilarResponseModel();
        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(_apiTimeoutSecond);

            var url = $"{baseUrl}/question/benzer";
            if (baseUrl.Contains("185.195.255.124")) url = $"{baseUrl}/Benzer";

            var data = new QuestionRequestModel
            {
                QuestionImage = model.Base64,
                LessonName = model.LessonName?.Trim().ToLower().ReplaceTurkishToLatin().Replace(" ", "_") ?? string.Empty
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{url}/Benzer")
            {
                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                similar = JsonSerializer.Deserialize<SimilarResponseModel>(content, _options) ?? throw new ExternalApiException(Strings.DynamicNotNull, nameof(similar));

                if (similar.QuestionText.EmptyOrTrim().Contains("Soru algılanamadı. Lütfen tekrar deneyiniz.", StringComparison.OrdinalIgnoreCase))
                {
                    await InfrastructureDelegates.UpdateSimilarAnswer?.Invoke(similar, new(model.Id, QuestionStatus.OcrError, model.UserId, baseUrl))!;
                    return similar;
                }

                if (!model.ExcludeQuiz)
                {
                    similar.RightOption = similar.RightOption.IsNotEmpty()
                        ? similar.RightOption.Trim("Cevap", ".", ":", ")", "-").ToUpper()[..1]
                        : throw new ExternalApiException(Strings.DynamicNotEmpty.Format(Strings.RightOption));

                    if (!_answersOptions.Contains(similar.RightOption, StringComparer.OrdinalIgnoreCase))
                        throw new ExternalApiException(Strings.DynamicBetween.Format(Strings.RightOption, "A", "E"));
                }

                similar.SimilarQuestionText = similar.SimilarQuestionText.EmptyOrTrim().Replace("Cevap X", string.Empty, StringComparison.OrdinalIgnoreCase);
                similar.AnswerText = similar.AnswerText.EmptyOrTrim().Replace("Cevap X", string.Empty, StringComparison.OrdinalIgnoreCase);

                if (InfrastructureDelegates.UpdateSimilarAnswer != null)
                    await InfrastructureDelegates.UpdateSimilarAnswer.Invoke(similar, new(model.Id, QuestionStatus.Answered, model.UserId, baseUrl));
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new ExternalApiException(content.Trim("{", "}"));
            }
        }
        catch (Exception ex)
        {
            var status = GetErrorQuestionStatus(ex);
            await InfrastructureDelegates.UpdateSimilarAnswer?.Invoke(similar, new(model.Id, status, model.UserId, baseUrl, ex.Message))!;
            LogError(loggerServiceBase, ex, model.UserId);
        }
        return similar;
    }

    public async Task<QuizResponseModel> GetQuizQuestions(QuizApiModel model)
    {
        var baseUrl = BaseUrl(model.AIUrl);
        var similars = new QuizResponseModel
        {
            Questions = []
        };

        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(_apiTimeoutSecond);

            var data = new QuizRequestModel
            {
                QuestionImages = model.QuestionImages,
                LessonName = model.LessonName?.Trim().ToLower() ?? string.Empty
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/Multi_Benzer")
            {
                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            similars = JsonSerializer.Deserialize<QuizResponseModel>(content, _options) ?? throw new ExternalApiException(Strings.DynamicNotNull, nameof(similars));
            similars.Questions!.ForEach(x =>
            {
                if (x != null)
                {
                    x.RightOption = x.RightOption.IsNotEmpty()
                        ? x.RightOption.Trim("Cevap", ":", ")", "-").ToUpper()
                        : throw new ExternalApiException(Strings.DynamicNotEmpty.Format(Strings.RightOption));

                    if (!_answersOptions.Contains(x.RightOption, StringComparer.OrdinalIgnoreCase))
                        throw new ExternalApiException(Strings.DynamicBetween.Format(Strings.RightOption, "A", "E"));
                }
            });
        }
        catch (Exception ex)
        {
            LogError(loggerServiceBase, ex, model.UserId);
        }

        return similars;
    }

    public async Task<QuizResponseModel> GetSimilarForQuiz(QuizApiModel model)
    {
        var baseUrl = BaseUrl(model.AIUrl);
        var similars = new QuizResponseModel
        {
            Questions = []
        };

        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(_apiTimeoutSecond);

            var message = string.Empty;
            for (int i = 0; i < model.QuestionImages?.Count; i++)
            {
                message = string.Empty;
                for (byte tryAgain = 0; tryAgain < 3; tryAgain++)
                {
                    Console.WriteLine($"{i + 1} - {tryAgain + 1} - {message}");

                    var question = model.QuestionImages[i];

                    var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/Model_Available");
                    var response = new HttpResponseMessage();

                    var count = (byte)0;
                    do
                    {
                        if (count > 0) await Task.Delay(3000);
                        response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                        count++;
                    }
                    while (response.StatusCode != HttpStatusCode.OK && count < 3);

                    var content = await response.Content.ReadAsStringAsync();
                    var available = Convert.ToBoolean(content);

                    if (!available) return similars;

                    var data = new QuestionRequestModel
                    {
                        QuestionImage = question,
                        LessonName = model.LessonName?.Trim().ToLower() ?? string.Empty
                    };

                    request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/Benzer")
                    {
                        Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
                    };

                    response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    content = await response.Content.ReadAsStringAsync();
                    var similar = JsonSerializer.Deserialize<SimilarResponseModel>(content, _options) ?? throw new ExternalApiException(Strings.DynamicNotNull, nameof(content));

                    if (similar.QuestionText.IsEmpty()
                        || similar.AnswerText.IsEmpty()
                        || _emptyQuestion.Any(x => similar.AnswerText.EmptyOrTrim().Contains(x, StringComparison.OrdinalIgnoreCase)))
                    {
                        if (tryAgain < _tryAgainCount) continue;
                        throw new ExternalApiException(Strings.DynamicNotEmpty, Strings.Question);
                    }

                    if (similar.RightOption.IsEmpty())
                    {
                        if (tryAgain < _tryAgainCount) continue;
                        throw new ExternalApiException(Strings.DynamicNotEmpty, Strings.RightOption);
                    }

                    similar.RightOption = similar.RightOption.Trim("Cevap", ".", ":", ")", "-").ToUpper()[..1];

                    if (!_answersOptions.Contains(similar.RightOption, StringComparer.OrdinalIgnoreCase))
                    {
                        if (tryAgain < _tryAgainCount) continue;
                        throw new ExternalApiException(Strings.DynamicBetween.Format(Strings.RightOption, "A", "E"));
                    }

                    similars.Questions.Add(similar);
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            LogError(loggerServiceBase, ex, model.UserId);
        }

        return similars;
    }
}