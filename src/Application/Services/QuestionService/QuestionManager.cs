using Application.Features.Lessons.Models.Gains;
using Application.Features.Questions.Models.Quizzes;
using Application.Features.Questions.Rules;
using Application.Services.CommonService;
using Application.Services.GainService;
using Application.Services.NotificationService;
using DataAccess.EF;
using Infrastructure.AI;
using Infrastructure.AI.Models;
using Infrastructure.AI.Seduss.Dtos;
using OneOf;

namespace Application.Services.QuestionService;

public class QuestionManager(ICommonService commonService,
                             INotificationService notificationService,
                             IDbContextFactory<HamsteraiDbContext> contextFactory,
                             IGainService gainService,
                             IQuestionApi questionApi,
                             ISimilarDal similarDal,
                             QuizRules quizRules) : IQuestionService
{
    public async Task SendQuestions(CancellationToken cancellationToken)
    {
        var methodName = nameof(SendQuestions);
        AppStatics.SenderQuestionAllow = false;
        try
        {
            Console.WriteLine($"{methodName} - Method Started: {DateTime.Now}");
            using var context = contextFactory.CreateDbContext();

            var allQuestions = await context.Questions
                .AsNoTracking()
                .Include(x => x.Lesson)
                .Where(x => AppStatics.QuestionStatusesForSender.Contains(x.Status)
                            && (!x.ManuelSendAgain || x.Status == QuestionStatus.Waiting)
                            && x.TryCount < AppOptions.AITryCount
                            && x.CreateDate > AppOptions.ChangeDate)
                .ToListAsync(cancellationToken);

            if (allQuestions.Count == 0) return;
            Console.WriteLine($"{methodName} - All Count: {allQuestions.Count}");
            var queueList = allQuestions.Take(AppOptions.AIQuestionQueueCapacity).ToList();
            Console.WriteLine($"{methodName} - Take Count: {queueList.Count}");

            var startDate = DateTime.Now;
            Console.WriteLine($"{methodName} - Start: {startDate}");

            var tasks = queueList.Select(async question =>
            {
                await AppStatics.QuestionSemaphore.WaitAsync(cancellationToken);
                try
                {
                    var base64 = string.Empty;
                    var questionPicturePath = Path.Combine(AppOptions.QuestionSmallPictureFolderPath, question.QuestionPictureFileName.EmptyOrTrim());
                    var questionSmallPicturePath = Path.Combine(AppOptions.QuestionSmallPictureFolderPath, question.QuestionPictureFileName.EmptyOrTrim());

                    if (File.Exists(questionSmallPicturePath))
                        base64 = await commonService.ImageToBase64WithResize(questionPicturePath, 512, cancellationToken);
                    else if (File.Exists(questionPicturePath))
                        base64 = await commonService.ImageToBase64WithResize(questionPicturePath, 512, cancellationToken);

                    if (base64.IsEmpty())
                    {
                        Console.WriteLine($"{question.Id},{QuestionStatus.NotFoundImage}, {question.CreateUser}, {string.Empty}, {Strings.DynamicNotFound.Format(Strings.Picture)}");
                        await UpdateQuestion(new QuestionResponseModel(), new UpdateQuestionDto(question.Id, QuestionStatus.NotFoundImage, question.CreateUser, question.LessonId, Strings.DynamicNotFound.Format(Strings.Picture)));
                        return;
                    }
                    var aiUrl = AppOptions.AIDefaultUrls.Length <= question.Lesson!.AIUrlIndex ? AppOptions.AIDefaultUrls[0] : AppOptions.AIDefaultUrls[question.Lesson!.AIUrlIndex];

                    var model = new QuestionApiModel
                    {
                        Id = question.Id,
                        LessonId = question.LessonId,
                        LessonName = question.Lesson!.Name,
                        UserId = question.CreateUser,
                        ExcludeQuiz = question.ExcludeQuiz,
                        Base64 = base64,
                        AIUrl = aiUrl,
                    };
                    Console.WriteLine($"{methodName} - Send: {DateTime.Now} -- {model.Id} -- Base64:{base64.Length} --");
                    await questionApi.AskQuestionWithImage(model);

                    model.AIUrl = AppOptions.AIDefaultUrls[2];
                    var visual = await questionApi.IsExistsVisualContent(model, cancellationToken);
                    _ = await context.Questions
                        .Where(x => x.Id == question.Id)
                        .ExecuteUpdateAsync(x => x.SetProperty(p => p.ExistsVisualContent, visual), cancellationToken);
                }
                finally
                {
                    AppStatics.QuestionSemaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
            var endDate = DateTime.Now;
            Console.WriteLine($"{methodName} - End: {endDate}");
            Console.WriteLine($"{methodName} - Total Seconds: ********** {(endDate - startDate).TotalSeconds} s **********");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{methodName} - Error: {ex.Message}");
        }
        finally
        {
            Console.WriteLine($"{methodName} - Method Finished: {DateTime.Now}");
            AppStatics.SenderQuestionAllow = true;
        }
    }

    public async Task<bool> UpdateQuestion(QuestionResponseModel model, UpdateQuestionDto dto)
    {
        using var context = contextFactory.CreateDbContext();

        var data = await context.Questions
            .Include(x => x.Lesson)
            .FirstOrDefaultAsync(x => x.Id == dto.QuestionId && x.IsActive);
        await QuestionRules.QuestionShouldExists(data);

        string extension = string.Empty;
        string fileName = string.Empty;
        if (dto.Status == QuestionStatus.Answered)
        {
            extension = ".png";
            fileName = $"A_{dto.UserId}_{data!.LessonId}_{dto.QuestionId}{extension}";
            await commonService.TextToImage(model?.AnswerText, fileName, AppOptions.AnswerPictureFolderPath);
        }

        data!.UpdateUser = 1;
        data.UpdateDate = DateTime.Now;
        data.QuestionText = model?.QuestionText.Trim("--- OCR Start ---", "--- OCR End ---") ?? string.Empty;
        data.AnswerText = model?.AnswerText ?? string.Empty;
        data.AnswerPictureFileName = fileName ?? string.Empty;
        data.AnswerPictureExtension = extension ?? string.Empty;
        data.Status = dto.Status;
        data.GainId = null;
        data.RightOption = model?.RightOption?.FirstOrDefault();
        data.OcrMethod = model?.OcrMethod.IfNullEmptyString(string.Empty) ?? string.Empty;
        data.ErrorDescription = dto.ErrorMessage.IfNullEmptyString(string.Empty);
        data.AIIP = dto.AIIP;

        if (dto.Status is not QuestionStatus.Answered)
        {
            ++data.TryCount;
            data.Status = data.TryCount < AppOptions.AITryCount
                ? QuestionStatus.SendAgain
                : dto.Status == QuestionStatus.SendAgain
                    ? QuestionStatus.Error
                    : dto.Status;
        }

        context.Questions.Update(data);
        await context.SaveChangesAsync();

        Console.WriteLine($"Update: {DateTime.Now}  -- {data.Id} -- Status: {data.TryCount} ---- {data.Status} ---- {data.OcrMethod} ---- {data.ErrorDescription}");
        if (data.TryCount >= 3 && data.Status != QuestionStatus.Answered)
            Console.WriteLine($"Update: {DateTime.Now}  -- {data.Id} -- Will not be solved anymore ");

        if (dto.Status == QuestionStatus.Answered)
        {
            var datas = new Dictionary<string, string> {
                { "id", dto.QuestionId.ToString() },
                { "type", NotificationTypes.QuestionAnswered.ToString()},
            };
            _ = notificationService.PushNotificationByUserId(new(Strings.Answered, Strings.DynamicLessonQuestionAnswered.Format(data.Lesson?.Name!), NotificationTypes.QuestionAnswered, [data.CreateUser], datas, dto.QuestionId.ToString()));
        }

        return true;
    }

    public async Task SendSimilar(CancellationToken cancellationToken)
    {
        var methodName = nameof(SendSimilar);
        AppStatics.SenderSimilarAllow = false;
        try
        {
            Console.WriteLine($"{methodName} - Method Started: {DateTime.Now}");
            using var context = contextFactory.CreateDbContext();

            var allQuestions = await context.Questions
                .AsNoTracking()
                .Include(x => x.Lesson)
                .Where(x => !x.SendForQuiz && !x.ExcludeQuiz && x.IsRead
                          && x.SimilarId == null && !x.ExistsVisualContent
                          && x.Status == QuestionStatus.Answered
                          && x.QuestionText != string.Empty
                          && x.TryCount < AppOptions.AITryCount
                          && x.CreateDate > AppOptions.ChangeDate)
                .ToListAsync(cancellationToken);

            if (allQuestions.Count == 0) goto Quiz;
            Console.WriteLine($"{methodName} - All Count: {allQuestions.Count}");
            var queueList = allQuestions.Take(AppOptions.AISimilarQueueCapacity);
            Console.WriteLine($"{methodName} - Take Count: {queueList.Count()}");

            var startDate = DateTime.Now;
            Console.WriteLine($"{methodName} - Start: {startDate}");

            var tasks = queueList.Select(async question =>
            {
                await AppStatics.SimilarSemaphore.WaitAsync(cancellationToken);
                try
                {
                    var model = new QuestionApiModel
                    {
                        Id = question.Id,
                        LessonId = question.LessonId,
                        LessonName = question.Lesson!.Name,
                        UserId = question.CreateUser,
                        ExcludeQuiz = question.ExcludeQuiz,
                        QuestionText = question.QuestionText,
                        AIUrl = AppOptions.AIDefaultUrls[1]
                    };

                    Console.WriteLine($"{methodName} - Send: {DateTime.Now} -- {model.Id} --");
                    var response = await questionApi.GetSimilar(model);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{methodName} - {question.Id} - Error: {ex.Message}");
                }
                finally
                {
                    AppStatics.SimilarSemaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
            var endDate = DateTime.Now;
            Console.WriteLine($"{methodName} - End: {endDate}");
            Console.WriteLine($"{methodName} - Total Seconds: ********** {(endDate - startDate).TotalSeconds} s **********");

        Quiz:
            var quizList = await context.Similars
                        .Include(x => x.Gain)
                        .Include(x => x.Lesson)
                        .Where(x => x.Status == QuestionStatus.Answered
                                  && !x.SendForQuiz
                                  && x.CreateDate > AppOptions.ChangeDate
                                  && x.ResponseQuestionFileName != ""
                                  && x.ResponseAnswerFileName != "")
                        .GroupBy(x => new { x.CreateUser, x.LessonId })
                        .ToListAsync(cancellationToken);

            var date = DateTime.Now;
            await quizList.ForEachAsync(async group =>
            {
                var similarList = group
                    .Where(x => x.ResponseQuestionFileName.IsNotEmpty()
                               && File.Exists(Path.Combine(AppOptions.SimilarQuestionPictureFolderPath, x.ResponseQuestionFileName!)))
                    .OrderBy(x => x.CreateDate).Take(AppOptions.QuizMinimumQuestionLimit).ToList();

                if (similarList.Count >= AppOptions.QuizMinimumQuestionLimit)
                {
                    var quizModel = new AddQuizModel
                    {
                        LessonId = group.Key.LessonId,
                        UserId = group.Key.CreateUser,
                        QuestionList = similarList,
                        LessonName = similarList.First().Lesson!.Name
                    };
                    var quizId = await AddQuiz(quizModel, context, cancellationToken);

                    if (quizId.IsNotEmpty())
                    {
                        foreach (var similar in similarList)
                        {
                            similar.UpdateUser = 1;
                            similar.UpdateDate = date;
                            similar.IsRead = false;
                            similar.ReadDate = date;
                            similar.SendForQuiz = true;
                            similar.SendQuizDate = date;

                            await context.Questions.Where(x => x.Id == similar.Id)
                                .ExecuteUpdateAsync(x => x.SetProperty(p => p.SendForQuiz, true)
                                                          .SetProperty(p => p.SendQuizDate, date), cancellationToken);
                        }

                        context.Similars.UpdateRange(similarList);
                        await context.SaveChangesAsync(cancellationToken);
                    }
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{methodName} - Error: {ex.Message}");
        }
        finally
        {
            Console.WriteLine($"{methodName} - Method Finished: {DateTime.Now}");
            AppStatics.SenderSimilarAllow = true;
        }
    }

    public async Task<bool> AddSimilar(SimilarResponseModel model, UpdateQuestionDto dto)
    {
        using var context = contextFactory.CreateDbContext();

        await similarDal.DeleteAsync(dto.QuestionId);

        var date = DateTime.Now;

        if (dto.ErrorMessage.IsNotEmpty())
        {
            var question = await context.Questions.FirstAsync(x => x.Id == dto.QuestionId);
            question.UpdateUser = dto.UserId;
            question.UpdateDate = date;
            question.TryCount = (byte)(question.TryCount + 1);
            question.ErrorDescription = dto.ErrorMessage;
            context.Questions.Update(question);
            await context.SaveChangesAsync();
            return false;
        }

        var rightOption = model?.RightOption.IsNotEmpty() ?? false
                       ? model?.RightOption.Trim("Cevap", ".", ":", "(", ")", "-")!.ToUpper()[..1]
                       : throw new ExternalApiException(Strings.DynamicNotEmpty.Format(Strings.RightOption));
        var answerText = $"Cevap: {rightOption}";

        string extension = string.Empty, questionFileName = string.Empty, answerFileName = string.Empty;
        if (dto.Status == QuestionStatus.Answered)
        {
            extension = ".png";
            var fileName = $"{dto.UserId}_{dto.LessonId}_{dto.QuestionId}{extension}";
            questionFileName = $"SQ_{fileName}";
            answerFileName = $"SA_{fileName}";
            await commonService.TextToImage(model?.QuestionText, questionFileName, AppOptions.SimilarQuestionPictureFolderPath);
            await commonService.TextToImage(answerText, answerFileName, AppOptions.SimilarAnswerPictureFolderPath);
        }

        var data = new Similar
        {
            Id = dto.QuestionId,
            IsActive = true,
            CreateUser = dto.UserId,
            CreateDate = date,
            UpdateUser = dto.UserId,
            UpdateDate = date,
            LessonId = dto.LessonId,
            ResponseQuestion = model?.QuestionText ?? string.Empty,
            ResponseQuestionFileName = questionFileName,
            ResponseQuestionExtension = extension,
            ResponseAnswer = answerText,
            ResponseAnswerFileName = answerFileName,
            ResponseAnswerExtension = extension,
            Status = dto.Status,
            IsRead = false,
            ReadDate = AppStatics.MilleniumDate,
            SendForQuiz = false,
            SendQuizDate = AppStatics.MilleniumDate,
            TryCount = 0,
            GainId = null,
            RightOption = rightOption?.FirstOrDefault(),
            ExcludeQuiz = false,
            ExistsVisualContent = false,
        };

        if (dto.Status is not QuestionStatus.Answered)
        {
            ++data.TryCount;
            data.Status = data.TryCount < AppOptions.AITryCount
                ? QuestionStatus.SendAgain
                : dto.Status == QuestionStatus.SendAgain
                    ? QuestionStatus.Error
                    : dto.Status;
        }

        context.Similars.Add(data);

        if (data.RightOption != null && data.ResponseQuestionFileName.IsNotEmpty())
        {
            var question = await context.Questions.FirstAsync(x => x.Id == dto.QuestionId);

            question.UpdateUser = dto.UserId;
            question.UpdateDate = date;
            question.SimilarId = data.Id;

            context.Questions.Update(question);
        }

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<string> AddQuiz(AddQuizModel model, HamsteraiDbContext? context, CancellationToken cancellationToken)
    {
        var methodName = nameof(AddQuiz);

        Console.WriteLine($"{methodName} - Method Started: {DateTime.Now}");
        await QuizRules.QuizQuestionShouldExists(model.QuestionList);

        var isNull = context == null;
        context ??= contextFactory.CreateDbContext();

        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        var userId = 1;
        var date = DateTime.Now;
        var idPrefix = $"T-{model.LessonId}-{model.UserId}-";
        var quziId = $"{idPrefix}{date:HHmmssfff}";

        try
        {
            await quizRules.QuizQuestionShouldNotExistsById(quziId);
            var quiz = new Quiz
            {
                Id = quziId,
                IsActive = true,
                CreateUser = userId,
                CreateDate = date,
                UpdateUser = userId,
                UpdateDate = date,
                UserId = model.UserId,
                LessonId = model.LessonId,
                TimeSecond = 0,
                Status = QuizStatus.NotStarted,
                CorrectCount = 0,
                WrongCount = 0,
                EmptyCount = 0,
                SuccessRate = 0,
            };

            var questions = new List<QuizQuestion>();

            for (byte i = 0; i < model.QuestionList.Count; i++)
            {
                var similar = model.QuestionList[i];
                var sortNo = (byte)(i + 1);

                var questionId = $"{quiz.Id}-{sortNo}";
                var extension = ".png";
                var fileName = $"{model.UserId}_{model.LessonId}_{questionId}{extension}";
                var questionFileName = $"TQ_{fileName}";
                var questionSourcePath = Path.Combine(AppOptions.SimilarQuestionPictureFolderPath, similar.ResponseQuestionFileName!);
                var questionTargetPath = Path.Combine(AppOptions.QuizQuestionPictureFolderPath, questionFileName);
                var answerFileName = $"TA_{fileName}";
                var answerSourcePath = Path.Combine(AppOptions.SimilarAnswerPictureFolderPath, similar.ResponseAnswerFileName!);
                var answerTargetPath = Path.Combine(AppOptions.QuizAnswerPictureFolderPath, answerFileName);
                if (!File.Exists(questionSourcePath)) throw new BusinessException(Strings.DynamicNotFound.Format(questionSourcePath));
                if (!File.Exists(answerSourcePath)) throw new BusinessException(Strings.DynamicNotFound.Format(answerSourcePath));
                File.Copy(questionSourcePath, questionTargetPath, true);
                File.Copy(answerSourcePath, answerTargetPath, true);

                questions.Add(new()
                {
                    Id = questionId,
                    IsActive = true,
                    CreateUser = userId,
                    CreateDate = date,
                    UpdateUser = userId,
                    UpdateDate = date,
                    QuizId = quiz.Id,
                    SortNo = sortNo,
                    Question = similar.ResponseQuestion ?? string.Empty,
                    QuestionPictureFileName = questionFileName ?? string.Empty,
                    QuestionPictureExtension = extension ?? string.Empty,
                    Answer = similar.ResponseAnswer ?? string.Empty,
                    AnswerPictureFileName = answerFileName ?? string.Empty,
                    AnswerPictureExtension = extension ?? string.Empty,
                    RightOption = similar.RightOption ?? 'A',
                    AnswerOption = null,
                    OptionCount = (byte)AppStatics.OptionsWithParentheses.Count((similar.ResponseQuestion ?? string.Empty).Contains),
                    GainId = similar.GainId,
                });
            }

            await context.Quizzes.AddAsync(quiz, cancellationToken);
            await context.QuizQuestions.AddRangeAsync(questions, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            var datas = new Dictionary<string, string> {
                { "id", quiz.Id },
                { "type", NotificationTypes.QuizCreated.ToString()},
            };
            _ = notificationService.PushNotificationByUserId(new(Strings.DynamicLessonTestPrepared.Format(model.LessonName!), Strings.DynamicLessonTestPreparedForYou.Format(model.LessonName!, quiz.Id), NotificationTypes.QuizCreated, [model.UserId], datas, quiz.Id));

            return quiz.Id;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            Console.WriteLine($"{methodName} - Error: {ex.Message}");
            throw new BusinessException(ex.Message);
        }
        finally
        {
            if (isNull) context?.Dispose();
            Console.WriteLine($"{methodName} - Method Finished: {DateTime.Now}");
        }
    }

    public async Task SendGain(CancellationToken cancellationToken)
    {
        var methodName = nameof(SendGain);
        AppStatics.SenderGainAllow = false;
        try
        {
            Console.WriteLine($"{methodName} - Method Started: {DateTime.Now}");

            using var context = contextFactory.CreateDbContext();

            var questions = await context.Questions
                .AsNoTracking()
                .Include(x => x.Lesson)
                .Where(x => x.Status == QuestionStatus.Answered && x.GainId == null && x.TryCount < AppOptions.AITryCount && x.CreateDate > AppOptions.ChangeDate && x.QuestionText != string.Empty)
                .ToListAsync(cancellationToken);

            var similarQuestions = await context.Similars
                .AsNoTracking()
                .Include(x => x.Lesson)
                .Where(x => x.Status == QuestionStatus.Answered && x.GainId == null && x.TryCount < AppOptions.AITryCount && x.CreateDate > AppOptions.ChangeDate && x.ResponseQuestion != string.Empty)
                .ToListAsync(cancellationToken);

            var allQuestions = questions.Select(OneOf<Question, Similar>.FromT0)
                                        .Concat(similarQuestions.Select(OneOf<Question, Similar>.FromT1))
                                        .ToList();

            if (allQuestions.Count == 0) return;
            Console.WriteLine($"{methodName} - All Count: {allQuestions.Count}");
            var queueList = allQuestions.Take(AppOptions.AIGainQueueCapacity).ToList();
            Console.WriteLine($"{methodName} - Take Count: {queueList.Count}");

            var startDate = DateTime.Now;
            Console.WriteLine($"{methodName} - Start: {startDate}");

            var tasks = queueList.Select(async question =>
            {
                await AppStatics.GainSemaphore.WaitAsync(cancellationToken);
                try
                {
                    if (question.IsT0)
                    {
                        var model = new QuestionApiModel
                        {
                            Id = question.AsT0.Id,
                            LessonName = question.AsT0.Lesson!.Name,
                            QuestionText = question.AsT0.QuestionText,
                            UserId = question.AsT0.CreateUser,
                            ExcludeQuiz = question.AsT0.ExcludeQuiz,
                            AIUrl = AppOptions.AIDefaultUrls[3],
                            LessonId = question.AsT0.LessonId
                        };

                        Console.WriteLine($"{methodName} - SendForQuestion: {DateTime.Now} -- {model.Id} -- Lenght:{question.AsT0.QuestionText?.Length} --");
                        var response = await questionApi.GetGain(model);

                        var gain = await gainService.GetOrAddGain(new(response.GainName, question.AsT0.LessonId, question.AsT0.CreateUser, context));

                        var data = context.Questions.Where(x => x.Id == question.AsT0.Id);
                        if (gain == null || gain.Id <= 0 || gain.Name.IsEmpty())
                            await data.ExecuteUpdateAsync(x => x.SetProperty(p => p.TryCount, question.AsT0.TryCount + 1));
                        else
                            await data.ExecuteUpdateAsync(x => x.SetProperty(p => p.GainId, gain.Id));
                    }

                    if (question.IsT1)
                    {
                        var model = new QuestionApiModel
                        {
                            Id = question.AsT1.Id,
                            LessonName = question.AsT1.Lesson!.Name,
                            QuestionText = question.AsT1.ResponseQuestion,
                            UserId = question.AsT1.CreateUser,
                            ExcludeQuiz = question.AsT1.ExcludeQuiz,
                            AIUrl = AppOptions.AIDefaultUrls[3],
                            LessonId = question.AsT1.LessonId,
                        };

                        Console.WriteLine($"{methodName} - SendForSimilar: {DateTime.Now} -- {model.Id} -- Lenght:{question.AsT1.ResponseQuestion?.Length} --");
                        var response = await questionApi.GetGain(model);

                        var gain = await gainService.GetOrAddGain(new(response.GainName, question.AsT1.LessonId, question.AsT1.CreateUser, context));

                        var data = context.Similars.Where(x => x.Id == question.AsT1.Id);
                        if (gain == null || gain.Id <= 0 || gain.Name.IsEmpty())
                            await data.ExecuteUpdateAsync(x => x.SetProperty(p => p.TryCount, question.AsT1.TryCount + 1));
                        else
                            await data.ExecuteUpdateAsync(x => x.SetProperty(p => p.GainId, gain.Id));
                    }
                }
                finally
                {
                    AppStatics.GainSemaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
            var endDate = DateTime.Now;
            Console.WriteLine($"{methodName} - End: {endDate}");
            Console.WriteLine($"{methodName} - Total Seconds: ********** {(endDate - startDate).TotalSeconds} s **********");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{methodName} - Error: {ex.Message}");
        }
        finally
        {
            Console.WriteLine($"{methodName} - Method Finished: {DateTime.Now}");
            AppStatics.SenderGainAllow = true;
        }
    }
}