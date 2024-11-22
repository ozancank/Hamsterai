using Amazon.Runtime.Internal.Transform;
using Application.Features.Lessons.Models.Gains;
using Application.Features.Questions.Models.Quizzes;
using Application.Features.Questions.Rules;
using Application.Features.Users.Rules;
using Application.Services.CommonService;
using Application.Services.GainService;
using Application.Services.NotificationService;
using DataAccess.EF;
using Infrastructure.AI;
using Infrastructure.AI.Seduss.Dtos;
using Infrastructure.AI.Seduss.Models;
using OCK.Core.Interfaces;

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
        var changeDate = new DateTime(2024, 11, 17, 0, 0, 0);
        try
        {
            Console.WriteLine($"{methodName} - Method Started: {DateTime.Now}");
            QuestionStatus[] status = [QuestionStatus.Waiting, QuestionStatus.Error, QuestionStatus.SendAgain, QuestionStatus.ConnectionError, QuestionStatus.Timeout];
            using var context = contextFactory.CreateDbContext();

            var allQuestions = await context.Questions
                .AsNoTracking()
                .Include(x => x.Lesson)
                .Where(x => status.Contains(x.Status) && x.TryCount < AppOptions.AITryCount && x.CreateDate > changeDate)
                .ToListAsync(cancellationToken);

            if (allQuestions.Count == 0) return;
            Console.WriteLine($"{methodName} - All Count: {allQuestions.Count}");
            var queueList = allQuestions.Take(AppOptions.QuestionQueueCapacity).ToList();
            Console.WriteLine($"{methodName} - Take Count: {queueList.Count}");

            var startDate = DateTime.Now;
            Console.WriteLine($"{methodName} - Start: {startDate}");

            var tasks = queueList.Select(async question =>
            {
                await AppStatics.QuestionSemaphore.WaitAsync(cancellationToken);
                try
                {
                    var base64 = await commonService.ImageToBase64(
                        Path.Combine(AppOptions.QuestionPictureFolderPath, question.QuestionPictureFileName.EmptyOrTrim()));

                    if (base64.IsEmpty())
                    {
                        Console.WriteLine($"{question.Id},{QuestionStatus.NotFoundImage}, {question.CreateUser}, {string.Empty}, {Strings.DynamicNotFound.Format(Strings.Picture)}");

                        await UpdateQuestion(new QuestionResponseModel(), new UpdateQuestionDto(question.Id, QuestionStatus.NotFoundImage, question.CreateUser, question.LessonId, Strings.DynamicNotFound.Format(Strings.Picture)));
                        return;
                    }
                    var aiUrl = AppOptions.AIDefaultUrls.Length <= question.Lesson!.AIUrlIndex ? AppOptions.AIDefaultUrls[0]: AppOptions.AIDefaultUrls[question.Lesson!.AIUrlIndex] ;

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
                    Console.WriteLine($"{methodName} - Send: {DateTime.Now} -- {model.Id} --");
                    await questionApi.AskQuestionWithImage(model);
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

    private async Task UpdateUserCredit(long userId, int credit, bool increase)
    {
        using var context = contextFactory.CreateDbContext();
        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        await UserRules.UserShouldExistsAndActive(user);
        if (increase) user!.AddtionalCredit += credit;
        else
        {
            if (user!.PackageCredit > 0)
                user.AddtionalCredit -= credit;
            else if (user.AddtionalCredit > 0)
                user.PackageCredit -= credit;
            else
                throw new BusinessException(Strings.CreditNotEnough);
        }
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task<bool> UpdateQuestion(QuestionResponseModel model, UpdateQuestionDto dto)
    {
        using var context = contextFactory.CreateDbContext();

        var data = await context.Questions
            .Include(x => x.Lesson)
            .FirstOrDefaultAsync(x => x.Id == dto.QuestionId && x.IsActive);
        await QuestionRules.QuestionShouldExists(data);

        GetGainModel? gain = null;
        //if (dto.Status == QuestionStatus.Answered && model.GainName.IsNotEmpty())
        //    gain = await gainService.GetOrAddGain(new(model?.GainName, data!.LessonId, data.CreateUser, context));

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
        data.QuestionPictureBase64 = model?.QuestionText.Trim("--- OCR Start ---", "--- OCR End ---") ?? string.Empty;
        data.AnswerText = model?.AnswerText ?? string.Empty;
        data.AnswerPictureFileName = fileName ?? string.Empty;
        data.AnswerPictureExtension = extension ?? string.Empty;
        data.Status = dto.Status;
        data.GainId = gain?.Id;
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

            if (data.TryCount >= AppOptions.AITryCount)
                await UpdateUserCredit(data.CreateUser, 1, true);
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
            _ = notificationService.PushNotificationByUserId(new(Strings.Answered, Strings.DynamicLessonQuestionAnswered.Format(data.Lesson?.Name), NotificationTypes.QuestionAnswered, [data.CreateUser], datas, dto.QuestionId.ToString()));
        }

        return true;
    }

    public async Task SendSimilar(CancellationToken cancellationToken)
    {
        var methodName = nameof(SendSimilar);
        AppStatics.SenderSimilarAllow = false;
        var changeDate = new DateTime(2024, 11, 19, 17, 0, 0);
        try
        {
            Console.WriteLine($"{methodName} - Method Started: {DateTime.Now}");
            using var context = contextFactory.CreateDbContext();

            var allQuestions = await context.Questions
                .AsNoTracking()
                .Include(x => x.Lesson)
                .Where(x => !x.SendForQuiz && !x.ExcludeQuiz
                          && x.Status == QuestionStatus.Answered
                          && x.QuestionPictureBase64 != string.Empty
                          && x.TryCount < AppOptions.AITryCount
                          && x.CreateDate > changeDate)
                .ToListAsync(cancellationToken);

            if (allQuestions.Count == 0) goto Quiz;
            Console.WriteLine($"{methodName} - All Count: {allQuestions.Count}");
            var queueList = allQuestions.Take(AppOptions.SimilarQueueCapacity);
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
                        QuestionText = question.QuestionPictureBase64,
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
                                  && x.CreateDate > changeDate
                                  && x.ResponseAnswerFileName != ""
                                  && x.ResponseAnswerFileName != "")
                        .GroupBy(x => new { x.CreateUser, x.LessonId })
                        .ToListAsync(cancellationToken);

            await quizList.ForEachAsync(async group =>
            {
                var quizList = group.OrderBy(x => x.CreateDate).Take(AppOptions.QuizMinimumQuestionLimit).ToList();

                if (quizList.Count >= AppOptions.QuizMinimumQuestionLimit)
                {
                    var quizModel = new AddQuizModel
                    {
                        LessonId = group.Key.LessonId,
                        UserId = group.Key.CreateUser,
                        QuestionList = quizList,
                        LessonName = quizList.First().Lesson!.Name
                    };
                    var quizId = await AddQuiz(quizModel, context, cancellationToken);

                    if (quizId.IsNotEmpty())
                    {
                        foreach (var quiz in quizList)
                        {
                            quiz.SendForQuiz = true;
                        }

                        context.Similars.UpdateRange(quizList);
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

    public async Task<bool> AddSimilarQuestion(SimilarResponseModel model, UpdateQuestionDto dto)
    {
        using var context = contextFactory.CreateDbContext();

        await similarDal.DeleteAsync(dto.QuestionId);

        var date = DateTime.Now;
        GetGainModel? gain = null;
        if (dto.Status == QuestionStatus.Answered && model.GainName.IsNotEmpty())
            gain = await gainService.GetOrAddGain(new(model?.GainName, dto.LessonId, dto.UserId, context));

        var rightOption = model?.RightOption.IsNotEmpty() ?? false
                       ? model?.RightOption.Trim("Cevap", ".", ":", "(", ")", "-").ToUpper()[..1]
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
            QuestionPicture = string.Empty,
            QuestionPictureFileName = string.Empty,
            QuestionPictureExtension = string.Empty,
            ResponseQuestion = model?.QuestionText ?? string.Empty,
            ResponseQuestionFileName = questionFileName,
            ResponseQuestionExtension = extension,
            ResponseAnswer = answerText,
            ResponseAnswerFileName = answerFileName,
            ResponseAnswerExtension = extension,
            Status = dto.Status,
            IsRead = false,
            SendForQuiz = false,
            TryCount = 0,
            GainId = gain?.Id,
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
            question.GainId = gain?.Id;
            question.SendForQuiz = true;
            question.RightOption = data.RightOption;

            context.Questions.Update(question);
        }

        await context.SaveChangesAsync();
        return true;
    }

    private static byte GetOptionCount(string questionText)
    {
        string[] options = ["A) ", "B) ", "C) ", "D) ", "E) "];
        return (byte)options.Count(questionText.Contains);
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
                    OptionCount = GetOptionCount(similar.ResponseQuestion ?? string.Empty),
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
            _ = notificationService.PushNotificationByUserId(new(Strings.DynamicLessonTestPrepared.Format(model.LessonName), Strings.DynamicLessonTestPreparedForYou.Format(model.LessonName, quiz.Id), NotificationTypes.QuizCreated, [model.UserId], datas, quiz.Id));

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

    /*
    //public async Task<bool> UpdateSimilarAnswer(SimilarResponseModel model, UpdateQuestionDto dto)
    //{
    //    using var context = contextFactory.CreateDbContext();

    //    var data = await context.Similars
    //        .Include(x => x.Lesson)
    //        .FirstOrDefaultAsync(x => x.Id == dto.QuestionId && x.IsActive);
    //    await SimilarRules.SimilarQuestionShouldExists(data);

    //    GetGainModel? gain = null;
    //    if (dto.Status == QuestionStatus.Answered && model.GainName.IsNotEmpty())
    //        gain = await gainService.GetOrAddGain(new(model?.GainName, data!.LessonId, data.CreateUser, context));

    //    string extension = string.Empty, questionFileName = string.Empty, answerFileName = string.Empty;
    //    if (dto.Status == QuestionStatus.Answered)
    //    {
    //        extension = ".png";
    //        var fileName = $"{dto.UserId}_{data!.LessonId}_{dto.QuestionId}{extension}";
    //        questionFileName = $"SQ_{fileName}";
    //        answerFileName = $"SA_{fileName}";
    //        await commonService.PictureConvert(model?.SimilarImage, questionFileName, AppOptions.SimilarQuestionPictureFolderPath);
    //        await commonService.PictureConvert(model?.AnswerImage, answerFileName, AppOptions.SimilarAnswerPictureFolderPath);
    //    }

    //    data!.UpdateUser = 1;
    //    data.UpdateDate = DateTime.Now;
    //    data.QuestionPicture = model?.QuestionText.Trim("--- OCR Start ---", "--- OCR End ---") ?? string.Empty;
    //    data.ResponseQuestion = model?.SimilarQuestionText ?? string.Empty;
    //    data.ResponseQuestionFileName = questionFileName ?? string.Empty;
    //    data.ResponseQuestionExtension = extension ?? string.Empty;
    //    data.ResponseAnswer = model?.AnswerText ?? string.Empty;
    //    data.ResponseAnswerFileName = answerFileName ?? string.Empty;
    //    data.ResponseAnswerExtension = extension ?? string.Empty;
    //    data.Status = dto.Status;
    //    data.GainId = gain?.Id;
    //    data.RightOption = model?.RightOption?.FirstOrDefault();
    //    data.OcrMethod = model?.OcrMethod.IfNullEmptyString(string.Empty) ?? string.Empty;
    //    data.ErrorDescription = dto.ErrorMessage.IfNullEmptyString(string.Empty);
    //    data.AIIP = dto.AIIP;
    //    if (dto.Status is not QuestionStatus.Answered)
    //    {
    //        data.TryCount++;
    //        data.Status = data.TryCount < AppOptions.AITryCount
    //            ? QuestionStatus.SendAgain
    //            : dto.Status == QuestionStatus.SendAgain
    //                ? QuestionStatus.Error
    //                : dto.Status;

    //        if (data.TryCount >= AppOptions.AITryCount)
    //            await UpdateUserCredit(data.CreateUser, 1, true);
    //    }

    //    context.Similars.Update(data);
    //    await context.SaveChangesAsync();

    //    if (dto.Status == QuestionStatus.Answered)
    //        _ = notificationService.PushNotificationByUserId(new(Strings.Prepared, Strings.DynamicLessonQuestionPrepared.Format(data.Lesson!.Name), data.CreateUser, NotificationTypes.SimilarCreated, data.Id.ToString()));

    //    return true;
    //}
    */

    //public async Task<bool> AddQuiz(bool timePass = false, CancellationToken cancellationToken = default)
    //{
    //    try
    //    {
    //        AppStatics.SenderQuestionAllow = false;
    //        using var context = contextFactory.CreateDbContext();

    //        var questions = await context.Questions
    //            .AsNoTracking()
    //            .Include(u => u.User).ThenInclude(u => u!.School)
    //            .Include(u => u.Lesson)
    //            .Include(u => u.Gain)
    //            .Where(x => x.IsActive
    //                        && x.Status == QuestionStatus.Answered
    //                        && !x.SendForQuiz
    //                        && !x.ExcludeQuiz
    //                        && x.User!.IsActive
    //                        && x.Lesson!.IsActive
    //                        && x.Gain!.IsActive
    //                        && (x.User.SchoolId == null || x.User.School!.IsActive)
    //                        && (x.User.SchoolId == null || x.User.School!.LicenseEndDate.Date >= DateTime.Now.Date))
    //            .Select(x => new { x.Id, x.QuestionPictureBase64, x.QuestionPictureFileName, x.CreateUser, x.User!.SchoolId, x.LessonId, x.ExistsVisualContent })
    //            .ToListAsync(cancellationToken);

    //        if (questions.Count == 0) return false;

    //        var groupedQuestions = questions
    //            .GroupBy(x => new { x.SchoolId, x.LessonId, x.CreateUser })
    //            .Where(x => x.Count() > AppOptions.QuizMinimumQuestionLimit)
    //            .SelectMany(x => x.Take(AppOptions.QuizMinimumQuestionLimit).Select(q => new
    //            {
    //                q.Id,
    //                q.QuestionPictureFileName,
    //                q.CreateUser,
    //                q.SchoolId,
    //                q.LessonId,
    //                q.ExistsVisualContent
    //            }))
    //            .OrderBy(o => o.CreateUser).ThenBy(o => o.SchoolId).ThenBy(o => o.LessonId).ThenBy(o => o.CreateUser)
    //            .ToList();

    //        if (groupedQuestions.Count == 0) return false;

    //        var userGroups = groupedQuestions.GroupBy(q => q.CreateUser);

    //        foreach (var userGroup in userGroups)
    //        {
    //            var lessonGroups = userGroup.GroupBy(q => q.LessonId);

    //            foreach (var lessonGroup in lessonGroups)
    //            {
    //                if (cancellationToken.IsCancellationRequested)
    //                    return false;

    //                if (DateTime.Now.Hour >= 7 && !timePass) return false;

    //                var base64List = new List<string>();
    //                var visualList = new List<bool>();

    //                var questionsIds = lessonGroup.Select(x => x.Id).ToList();

    //                foreach (var question in lessonGroup)
    //                {
    //                    var filePath = Path.Combine(AppOptions.QuestionPictureFolderPath, question.QuestionPictureFileName!);
    //                    var base64String = await commonService.ImageToBase64(filePath);
    //                    base64List.Add(base64String);
    //                    visualList.Add(question.ExistsVisualContent);
    //                }

    //                var addQuizModel = new AddQuizModel
    //                {
    //                    QuestionList = base64List,
    //                    LessonId = lessonGroup.Key,
    //                    UserId = userGroup.Key,
    //                    VisualList = visualList
    //                };

    //                try
    //                {
    //                    await AddQuiz(addQuizModel, cancellationToken);

    //                    foreach (var questionId in questionsIds)
    //                    {
    //                        var questionUpdate = await context.Questions.FirstOrDefaultAsync(x => x.Id == questionId, cancellationToken);
    //                        await QuestionRules.QuestionShouldExists(questionUpdate);
    //                        questionUpdate!.SendForQuiz = true;
    //                        context.Questions.Update(questionUpdate);
    //                        await context.SaveChangesAsync(cancellationToken);
    //                    }
    //                }
    //                catch (Exception ex)
    //                {
    //                    logger.Error($"{addQuizModel.UserId} - {addQuizModel.LessonId} - {ex.Message}");
    //                    continue;
    //                }
    //            }
    //        }

    //        return true;
    //    }
    //    finally
    //    {
    //        AppStatics.SenderQuestionAllow = true;
    //    }
    //}
}