using Domain.Constants;
using Infrastructure.AI.Seduss.Dtos;
using Infrastructure.AI.Seduss.Models;
using Infrastructure.Constants;
using OCK.Core.Exceptions.CustomExceptions;
using OCK.Core.Logging.Serilog;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Infrastructure.AI.Seduss;

public class SedussApi(IHttpClientFactory httpClientFactory, LoggerServiceBase loggerServiceBase) : IQuestionApi
{
    private static readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
    private const double _apiTimeoutMinute = 1;
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

    private static string BaseUrl(long userId)
    {
        return userId switch
        {
            18 => AppOptions.AI_Kazim1,
            19 => AppOptions.AI_Kazim2,
            20 => AppOptions.AI_Kazim3,
            _ => AppOptions.AI_Default
        };
    }

    private static async Task<bool> ModelAvailable(string baseUrl, HttpClient client)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/Model_Available");
        var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        if (response.StatusCode != HttpStatusCode.OK) return false;
        var content = await response.Content.ReadAsStringAsync();
        var result = content == "1" ? "true" : content;
        return Convert.ToBoolean(result);
    }

    #region Question
    public async Task<QuestionITOResponseModel> AskQuestionOcrImage(QuestionApiModel model)
    {
        var baseUrl = BaseUrl(model.UserId);
        QuestionITOResponseModel answer = null;
        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(_apiTimeoutMinute);

            //var available = await ModelAvailable(baseUrl, client);

            //if (!available)
            //{
            //    answer = new QuestionITOResponseModel { QuestionText = model.Base64 };
            //    await InfrastructureDelegates.UpdateQuestionOcrImage.Invoke(answer, new(model.Id, QuestionStatus.SendAgain, model.UserId));
            //    return answer;
            //}

            var data = new QuestionRequestModel
            {
                QuestionImage = model.Base64,
                LessonName = model.LessonName?.Trim().ToLower() ?? string.Empty
            };

            var url = $"{baseUrl}/Soru_ITO";
            if (baseUrl.Contains("54.237.224.177")) url = $"{baseUrl}/question/sor";

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            answer = JsonSerializer.Deserialize<QuestionITOResponseModel>(content, _options);

            if (!model.ExcludeQuiz)
            {
                answer.RightOption = answer.RightOption.IsNotEmpty()
                    ? answer.RightOption.Trim("Cevap", ".", ":", ")", "-").ToUpper()[..1]
                    : throw new ExternalApiException(Strings.DynamicNotEmpty.Format(Strings.RightOption));

                if (!_answersOptions.Contains(answer.RightOption, StringComparer.OrdinalIgnoreCase))
                    throw new ExternalApiException(Strings.DynamicBetween.Format(Strings.RightOption, "A", "E"));
            }

            answer.AnswerText = answer.AnswerText.Replace("Cevap X", string.Empty, StringComparison.OrdinalIgnoreCase);

            if (InfrastructureDelegates.UpdateQuestionOcrImage != null)
                await InfrastructureDelegates.UpdateQuestionOcrImage.Invoke(answer, new(model.Id, QuestionStatus.Answered, model.UserId));
        }
        catch (Exception ex)
        {
            if (InfrastructureDelegates.UpdateQuestionOcrImage != null)
                await InfrastructureDelegates.UpdateQuestionOcrImage.Invoke(answer, new(model.Id, QuestionStatus.Error, model.UserId));
            loggerServiceBase.Error($"AskQuestionOcrImage {model.UserId}*{ex?.InnerException?.Message}*{ex?.Message}*{ex?.StackTrace}");
        }

        return answer;
    }

    public async Task<QuestionTextResponseModel> AskQuestionText(QuestionApiModel model)
    {
        var baseUrl = BaseUrl(model.UserId);
        QuestionTextResponseModel answer = null;
        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(_apiTimeoutMinute);

            var available = await ModelAvailable(baseUrl, client);

            if (!available)
            {
                await InfrastructureDelegates.UpdateQuestionText?.Invoke(new(), new(model.Id, QuestionStatus.SendAgain, model.UserId));
                return answer;
            }

            var data = new QuestionTextRequestModel
            {
                QuestionText = model.QuestionText,
                LessonName = model.LessonName?.Trim().ToLower() ?? string.Empty
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/TextReq")
            {
                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            answer = JsonSerializer.Deserialize<QuestionTextResponseModel>(content, _options);

            if (model.QuestionText.IsEmpty()
                || answer.AnswerText.IsEmpty()
                || _emptyQuestion.Any(x => answer.AnswerText.Contains(x, StringComparison.OrdinalIgnoreCase)))
                throw new ExternalApiException(Strings.DynamicNotEmpty, Strings.Question);

            if (!model.ExcludeQuiz)
            {
                answer.RightOption = answer.RightOption.IsNotEmpty()
                    ? answer.RightOption.Trim("Cevap", ".", ":", ")", "-").ToUpper()[..1]
                    : throw new ExternalApiException(Strings.DynamicNotEmpty.Format(Strings.RightOption));

                if (!_answersOptions.Contains(answer.RightOption, StringComparer.OrdinalIgnoreCase))
                    throw new ExternalApiException(Strings.DynamicBetween.Format(Strings.RightOption, "A", "E"));
            }

            answer.AnswerText = answer.AnswerText.Replace("Cevap X", string.Empty, StringComparison.OrdinalIgnoreCase);

            if (InfrastructureDelegates.UpdateQuestionText != null)
                await InfrastructureDelegates.UpdateQuestionText?.Invoke(answer, new(model.Id, QuestionStatus.Answered, model.UserId));
        }
        catch (Exception ex)
        {
            await InfrastructureDelegates.UpdateQuestionText?.Invoke(answer, new(model.Id, QuestionStatus.Error, model.UserId));
            loggerServiceBase.Error($"AskQuestionText {model.UserId}*{ex?.InnerException?.Message}*{ex?.Message}*{ex?.StackTrace}");
        }
        return answer;
    }

    public async Task<QuestionVisualResponseModel> AskQuestionVisual(QuestionApiModel model)
    {
        var baseUrl = BaseUrl(model.UserId);
        QuestionVisualResponseModel answer = null;
        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(_apiTimeoutMinute);

            var available = await ModelAvailable(baseUrl, client);

            if (!available)
            {
                answer = new QuestionVisualResponseModel { QuestionText = model.Base64 };
                await InfrastructureDelegates.UpdateQuestionVisual.Invoke(answer, new(model.Id, QuestionStatus.SendAgain, model.UserId));
                return answer;
            }

            var data = new QuestionRequestModel
            {
                QuestionImage = model.Base64,
                LessonName = model.LessonName?.Trim().ToLower() ?? string.Empty
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/Gorsel_Soru")
            {
                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            answer = JsonSerializer.Deserialize<QuestionVisualResponseModel>(content, _options);

            //if (!model.ExcludeQuiz)
            //{
            //    answer.RightOption = answer.RightOption.IsNotEmpty()
            //        ? answer.RightOption.Trim("Cevap", ".", ":", ")", "-").ToUpper()[..1]
            //        : throw new ExternalApiException(Strings.DynamicNotEmpty.Format(Strings.RightOption));

            //    if (!_answersOptions.Contains(answer.RightOption, StringComparer.OrdinalIgnoreCase))
            //        throw new ExternalApiException(Strings.DynamicBetween.Format(Strings.RightOption, "A", "E"));
            //}

            answer.AnswerText = answer.AnswerText.Replace("Cevap X", string.Empty, StringComparison.OrdinalIgnoreCase);

            if (InfrastructureDelegates.UpdateQuestionOcrImage != null)
                await InfrastructureDelegates.UpdateQuestionVisual.Invoke(answer, new(model.Id, QuestionStatus.Answered, model.UserId));
        }
        catch (Exception ex)
        {
            if (InfrastructureDelegates.UpdateQuestionOcrImage != null)
                await InfrastructureDelegates.UpdateQuestionVisual.Invoke(answer, new(model.Id, QuestionStatus.Error, model.UserId));
            loggerServiceBase.Error($"AskQuestionVisual {model.UserId}*{ex?.InnerException?.Message}*{ex?.Message}*{ex?.StackTrace}");
        }

        return answer;
    }
    #endregion

    #region Similar
    public async Task<SimilarResponseModel> GetSimilar(QuestionApiModel model)
    {
        var baseUrl = BaseUrl(model.UserId);
        SimilarResponseModel similar = null;
        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(_apiTimeoutMinute);

            var available = await ModelAvailable(baseUrl, client);

            if (!available)
            {
                similar = new SimilarResponseModel { QuestionText = model.Base64 };
                await InfrastructureDelegates.UpdateSimilarAnswer.Invoke(new(), new UpdateQuestionDto(model.Id, QuestionStatus.SendAgain, model.UserId));
                return similar;
            }

            var data = new QuestionRequestModel
            {
                QuestionImage = model.Base64,
                LessonName = model.LessonName?.Trim().ToLower() ?? string.Empty
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/Benzer")
            {
                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            similar = JsonSerializer.Deserialize<SimilarResponseModel>(content, _options);

            if (!model.ExcludeQuiz)
            {
                similar.RightOption = similar.RightOption.IsNotEmpty()
                    ? similar.RightOption.Trim("Cevap", ".", ":", ")", "-").ToUpper()[..1]
                    : throw new ExternalApiException(Strings.DynamicNotEmpty.Format(Strings.RightOption));

                if (!_answersOptions.Contains(similar.RightOption, StringComparer.OrdinalIgnoreCase))
                    throw new ExternalApiException(Strings.DynamicBetween.Format(Strings.RightOption, "A", "E"));
            }

            similar.SimilarQuestionText = similar.SimilarQuestionText.Replace("Cevap X", string.Empty, StringComparison.OrdinalIgnoreCase);
            similar.AnswerText = similar.AnswerText.Replace("Cevap X", string.Empty, StringComparison.OrdinalIgnoreCase);

            if (InfrastructureDelegates.UpdateSimilarAnswer != null)
                await InfrastructureDelegates.UpdateSimilarAnswer.Invoke(similar, new(model.Id, QuestionStatus.Answered, model.UserId));
        }
        catch (Exception ex)
        {
            if (InfrastructureDelegates.UpdateSimilarAnswer != null)
                await InfrastructureDelegates.UpdateSimilarAnswer.Invoke(similar, new(model.Id, QuestionStatus.Error, model.UserId));
            loggerServiceBase.Error($"GetSimilar {model.UserId}*{ex?.InnerException?.Message}*{ex?.Message}*{ex?.StackTrace}");
        }
        return similar;
    }

    public async Task<SimilarTextResponseModel> GetSimilarText(QuestionApiModel model)
    {
        var baseUrl = BaseUrl(model.UserId);
        SimilarTextResponseModel similar = null;
        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(_apiTimeoutMinute);

            var available = await ModelAvailable(baseUrl, client);

            if (!available)
            {
                await InfrastructureDelegates.UpdateSimilarText.Invoke(new(), new UpdateQuestionDto(model.Id, QuestionStatus.SendAgain, model.UserId));
                return similar;
            }

            var data = new QuestionTextRequestModel
            {
                QuestionText = model.QuestionText,
                LessonName = model.LessonName?.Trim().ToLower() ?? string.Empty
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/Text2Benzer")
            {
                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            similar = JsonSerializer.Deserialize<SimilarTextResponseModel>(content, _options);

            if (model.QuestionText.IsEmpty()
            || similar.AnswerText.IsEmpty()
            || _emptyQuestion.Any(x => similar.AnswerText.Contains(x, StringComparison.OrdinalIgnoreCase)))
                throw new ExternalApiException(Strings.DynamicNotEmpty, Strings.Question);

            if (!model.ExcludeQuiz)
            {
                similar.RightOption = similar.RightOption.IsNotEmpty()
                    ? similar.RightOption.Trim("Cevap", ".", ":", ")", "-").ToUpper()[..1]
                    : throw new ExternalApiException(Strings.DynamicNotEmpty.Format(Strings.RightOption));

                if (!_answersOptions.Contains(similar.RightOption, StringComparer.OrdinalIgnoreCase))
                    throw new ExternalApiException(Strings.DynamicBetween.Format(Strings.RightOption, "A", "E"));
            }

            similar.SimilarQuestionText = similar.SimilarQuestionText.Replace("Cevap X", string.Empty, StringComparison.OrdinalIgnoreCase);
            similar.AnswerText = similar.AnswerText.Replace("Cevap X", string.Empty, StringComparison.OrdinalIgnoreCase);

            if (InfrastructureDelegates.UpdateSimilarText != null)
                await InfrastructureDelegates.UpdateSimilarText.Invoke(similar, new(model.Id, QuestionStatus.Answered, model.UserId));
        }
        catch (Exception ex)
        {
            if (InfrastructureDelegates.UpdateSimilarText != null)
                await InfrastructureDelegates.UpdateSimilarText.Invoke(similar, new(model.Id, QuestionStatus.Error, model.UserId));
            loggerServiceBase.Error($"GetSimilarText {model.UserId}*{ex?.InnerException?.Message}*{ex?.Message}*{ex?.StackTrace}");
        }
        return similar;
    }
    #endregion

    #region Quiz
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

            var available = await ModelAvailable(baseUrl, client);

            if (!available) return similars;

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
            loggerServiceBase.Error($"GetQuizQuestions {model.UserId}*{ex?.InnerException?.Message}*{ex?.Message}*{ex?.StackTrace}");
        }

        return similars;
    }

    public async Task<QuizResponseModel> GetSimilarForQuiz(QuizApiModel model)
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

            var message = string.Empty;
            for (int i = 0; i < model.QuestionImages.Count; i++)
            {
                message = string.Empty;
                for (byte tryAgain = 0; tryAgain < 3; tryAgain++)
                {
                    Console.WriteLine($"{i + 1} - {tryAgain + 1} - {message}");

                    var question = model.QuestionImages[i];

                    var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/Model_Available");
                    HttpResponseMessage response = null;

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
                    var similar = JsonSerializer.Deserialize<SimilarResponseModel>(content, _options);

                    if (similar.QuestionText.IsEmpty()
                        || similar.AnswerText.IsEmpty()
                        || _emptyQuestion.Any(x => similar.AnswerText.Contains(x, StringComparison.OrdinalIgnoreCase)))
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
            loggerServiceBase.Error($"GetSimilarForQuiz {model.UserId}*{ex?.InnerException?.Message}*{ex?.Message}*{ex?.StackTrace}");
        }

        return similars;
    }

    public async Task<QuizTextResponseModel> GetSimilarTextForQuiz(QuizApiModel model)
    {
        var baseUrl = BaseUrl(model.UserId);
        var similars = new QuizTextResponseModel
        {
            Questions = []
        };

        var message = string.Empty;
        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(_apiTimeoutMinute);

            for (int i = 0; i < model.QuestionTexts.Count; i++)
            {
                message = string.Empty;
                for (byte tryAgain = 0; tryAgain < 3; tryAgain++)
                {
                    Console.WriteLine($"{i + 1} - {tryAgain + 1} - {message}");

                    var question = model.QuestionTexts[i];

                    var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/Model_Available");
                    HttpResponseMessage response = null;

                    var count = (byte)0;
                    do
                    {
                        if (count > 0) await Task.Delay(2000);
                        response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                        count++;
                    }
                    while (response.StatusCode != HttpStatusCode.OK && count < 3);

                    var content = await response.Content.ReadAsStringAsync();
                    var available = Convert.ToBoolean(content);

                    if (!available) return similars;

                    var data = new QuestionTextRequestModel
                    {
                        QuestionText = question,
                        LessonName = model.LessonName?.Trim().ToLower() ?? string.Empty
                    };

                    var url = model.VisualList[i] ? "Benzer" : "Text2Benzer";

                    request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/{url}")
                    {
                        Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
                    };

                    response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    content = await response.Content.ReadAsStringAsync();
                    var similar = JsonSerializer.Deserialize<SimilarTextResponseModel>(content, _options);
                    similar.QuestionText = question;

                    if (similar.QuestionText.IsEmpty()
                        || similar.AnswerText.IsEmpty()
                        || _emptyQuestion.Any(x => similar.AnswerText.Contains(x, StringComparison.OrdinalIgnoreCase)))
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
            loggerServiceBase.Error($"GetSimilarTextForQuiz {model.UserId}*{ex?.InnerException?.Message}*{ex?.Message}*{ex?.StackTrace}");
        }

        return similars;
    }
    #endregion


}