using Application.Features.Lessons.Models.Gains;
using Application.Features.Lessons.Rules;
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
using Infrastructure.OCR;
using OCK.Core.Logging.Serilog;
using OneOf;

namespace Application.Services.QuestionService;

[Obsolete(message: "Currently Not Available")]
public class QuestionManager(ICommonService commonService,
                             INotificationService notificationService,
                             IGainService gainService,
                             IQuestionApi questionApi,
                             IDbContextFactory<HamsteraiDbContext> contextFactory,
                             IOcrApi ocrApi,
                             UserRules userRules,
                             QuizRules quizRules,
                             LoggerServiceBase logger) : IQuestionService
{
    public async Task SendQuestions(CancellationToken cancellationToken)
    {
        AppStatics.SenderQuestionAllow = false;
        var changeDate = new DateTime(2024, 10, 19, 1, 30, 0);
        try
        {
            QuestionStatus[] status = [QuestionStatus.Waiting, QuestionStatus.Error, QuestionStatus.SendAgain];
            using var context = contextFactory.CreateDbContext();

            var questions = await context.Questions
                .AsNoTracking()
                .Include(x => x.Lesson)
                .Where(x => status.Contains(x.Status) && x.TryCount < AppOptions.AITryCount && x.CreateDate > changeDate)
                .ToListAsync(cancellationToken);

            var similarQuestions = await context.Similars
                .AsNoTracking()
                .Include(x => x.Lesson)
                .Where(x => status.Contains(x.Status) && x.TryCount < AppOptions.AITryCount && x.CreateDate > changeDate)
                .ToListAsync(cancellationToken);

            var allQuestions = questions.Select(OneOf<Question, Similar>.FromT0)
                                        .Concat(similarQuestions.Select(OneOf<Question, Similar>.FromT1))
                                        .ToList();

            if (allQuestions.Count == 0) return;

            var tasks = allQuestions.Select(async question =>
            {
                await AppStatics.SenderSemaphore.WaitAsync(cancellationToken);
                try
                {
                    var result = await SendOcr(question, cancellationToken);

                    if (result.IsT0)
                    {
                        var model = new QuestionApiModel
                        {
                            Id = result.AsT0.Id,
                            LessonName = result.AsT0.Lesson!.Name,
                            UserId = result.AsT0.CreateUser,
                            ExcludeQuiz = result.AsT0.ExcludeQuiz
                        };

                        if (result.AsT0.ExistsVisualContent)
                        {
                            model.Base64 = result.AsT0.QuestionPictureBase64;
                            await questionApi.AskQuestionOcrImage(model);
                        }
                        else
                        {
                            model.QuestionText = result.AsT0.QuestionPictureBase64;
                            await questionApi.AskQuestionText(model);
                        }
                    }

                    if (result.IsT1)
                    {
                        var model = new QuestionApiModel
                        {
                            Id = result.AsT1.Id,
                            LessonName = result.AsT1.Lesson!.Name,
                            UserId = result.AsT1.CreateUser,
                            ExcludeQuiz = result.AsT1.ExcludeQuiz
                        };
                        if (result.AsT1.ExistsVisualContent)
                        {
                            model.Base64 = result.AsT1.QuestionPicture;
                            await questionApi.GetSimilar(model);
                        }
                        else
                        {
                            model.QuestionText = result.AsT1.QuestionPicture;
                            await questionApi.GetSimilarText(model);
                        }
                    }
                }
                finally
                {
                    AppStatics.SenderSemaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
        }
        finally
        {
            AppStatics.SenderQuestionAllow = true;
        }
    }

    private async Task<OneOf<Question, Similar>> SendOcr(OneOf<Question, Similar> entity, CancellationToken cancellationToken)
    {
        //return entity;

        using var context = contextFactory.CreateDbContext();

        var id = entity.Match(q => q.Id, s => s.Id);
        var userId = entity.Match(q => q.CreateUser, s => s.CreateUser);
        var username = await context.Users.Where(x => x.Id == userId).Select(x => x.UserName).FirstOrDefaultAsync(cancellationToken);
        var filePath = entity.Match(q => Path.Combine(AppOptions.QuestionPictureFolderPath, q.QuestionPictureFileName!), s => Path.Combine(AppOptions.QuestionPictureFolderPath, s.QuestionPictureFileName!));

        var ocr = await ocrApi.GetTextFromImage(new(filePath, username, userId));
        await QuestionRules.OCRShouldBeFilled(ocr);

        var existsVisualContent = ocr.Text!.Contains("##visual##", StringComparison.OrdinalIgnoreCase);
        var excludeQuiz = ocr.Text.Contains("##classic##", StringComparison.OrdinalIgnoreCase) || ocr.Text.Contains("Cevap X", StringComparison.OrdinalIgnoreCase);

        if (entity.IsT0)
        {
            var result = entity.AsT0;
            var data = await context.Questions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            result.QuestionPictureBase64 = data!.QuestionPictureBase64 = existsVisualContent
                ? data.QuestionPictureBase64
                : ocr.Text.Trim("##classic##", "##visual##");
            result.ExcludeQuiz = data.ExcludeQuiz = excludeQuiz;
            result.ExistsVisualContent = data.ExistsVisualContent = existsVisualContent;
            context.Questions.Update(data);
            await context.SaveChangesAsync(cancellationToken);
            return result;
        }
        else
        {
            var result = entity.AsT1;
            var data = await context.Similars.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            result.QuestionPicture = data!.QuestionPicture = existsVisualContent
                ? data.QuestionPicture
                : ocr.Text.Trim("##classic##", "##visual##");
            result.ExcludeQuiz = data.ExcludeQuiz = excludeQuiz;
            result.ExistsVisualContent = data.ExistsVisualContent = existsVisualContent;
            context.Similars.Update(data);
            await context.SaveChangesAsync(cancellationToken);
            return result;
        }
    }
    #region Question

    public async Task<bool> UpdateAnswer(QuestionITOResponseModel model, UpdateQuestionDto dto)
    {
        using var context = contextFactory.CreateDbContext();

        var data = await context.Questions
            .Include(x => x.Lesson)
            .FirstOrDefaultAsync(x => x.Id == dto.QuestionId && x.IsActive);
        await QuestionRules.QuestionShouldExists(data);

        GetGainModel? gain = null;
        if (dto.Status == QuestionStatus.Answered && model.GainName.IsNotEmpty())
            gain = await gainService.GetOrAddGain(new(model.GainName!, data!.LessonId, data.CreateUser, context));

        string extension = string.Empty;
        string fileName = string.Empty;
        if (dto.Status == QuestionStatus.Answered)
        {
            extension = ".png";
            fileName = $"A_{dto.UserId}_{data!.LessonId}_{dto.QuestionId}{extension}";
            await commonService.TextToImage(model?.AnswerText!, fileName, AppOptions.AnswerPictureFolderPath);
        }

        data!.UpdateUser = 1;
        data.UpdateDate = DateTime.Now;
        data.QuestionPictureBase64 = model?.QuestionText ?? string.Empty;
        data.AnswerText = model?.AnswerText ?? string.Empty;
        data.AnswerPictureFileName = fileName ?? string.Empty;
        data.AnswerPictureExtension = extension ?? string.Empty;
        data.Status = dto.Status;
        data.GainId = gain?.Id;
        data.RightOption = model?.RightOption?.FirstOrDefault();
        if (dto.Status != QuestionStatus.Answered)
        {
            data.TryCount++;
            data.Status = data.TryCount < AppOptions.AITryCount ? QuestionStatus.SendAgain : QuestionStatus.Error;
        }

        context.Questions.Update(data);
        await context.SaveChangesAsync();

        if (dto.Status == QuestionStatus.Answered)
            _ = notificationService.PushNotificationByUserId(new(Strings.Answered, Strings.DynamicLessonQuestionAnswered.Format(data.Lesson!.Name), data.CreateUser, NotificationTypes.QuestionAnswered, dto.QuestionId.ToString()));

        return true;
    }

    public async Task<bool> UpdateAnswer(QuestionTextResponseModel model, UpdateQuestionDto dto)
    {
        using var context = contextFactory.CreateDbContext();

        var data = await context.Questions
            .Include(x => x.Lesson)
            .FirstOrDefaultAsync(x => x.Id == dto.QuestionId && x.IsActive);
        await QuestionRules.QuestionShouldExists(data);

        GetGainModel? gain = null;
        if (dto.Status == QuestionStatus.Answered && model.GainName.IsNotEmpty())
            gain = await gainService.GetOrAddGain(new(model?.GainName!, data!.LessonId, data.CreateUser, context));

        string extension = string.Empty, fileName = string.Empty;
        if (dto.Status == QuestionStatus.Answered)
        {
            extension = ".png";
            fileName = $"A_{dto.UserId}_{data!.LessonId}_{dto.QuestionId}{extension}";
            await commonService.TextToImage(model?.AnswerText!, fileName, AppOptions.AnswerPictureFolderPath);
        }

        data!.UpdateUser = 1;
        data.UpdateDate = DateTime.Now;
        data.AnswerText = model?.AnswerText ?? string.Empty;
        data.AnswerPictureFileName = fileName ?? string.Empty;
        data.AnswerPictureExtension = extension ?? string.Empty;
        data.Status = dto.Status;
        data.GainId = gain?.Id;
        data.RightOption = model?.RightOption?.FirstOrDefault();
        if (dto.Status != QuestionStatus.Answered)
        {
            data.TryCount++;
            data.Status = data.TryCount < AppOptions.AITryCount ? QuestionStatus.SendAgain : QuestionStatus.Error;
        }

        context.Questions.Update(data);
        await context.SaveChangesAsync();

        if (dto.Status == QuestionStatus.Answered)
            _ = notificationService.PushNotificationByUserId(new(Strings.Answered, Strings.DynamicLessonQuestionAnswered.Format(data.Lesson!.Name), data.CreateUser, NotificationTypes.QuestionAnswered, dto.QuestionId.ToString()));

        return true;
    }

    public Task<bool> UpdateAnswer(QuestionVisualResponseModel model, UpdateQuestionDto dto)
    {
        throw new NotImplementedException();
    }

    #endregion Question

    #region SimilarQuestion

    public async Task<bool> UpdateSimilarAnswer(SimilarResponseModel model, UpdateQuestionDto dto)
    {
        using var context = contextFactory.CreateDbContext();

        var data = await context.Similars
            .Include(x => x.Lesson)
            .FirstOrDefaultAsync(x => x.Id == dto.QuestionId && x.IsActive);
        await SimilarRules.SimilarQuestionShouldExists(data);

        GetGainModel? gain = null;
        if (dto.Status == QuestionStatus.Answered && model.GainName.IsNotEmpty())
            gain = await gainService.GetOrAddGain(new(model?.GainName, data!.LessonId, data.CreateUser, context));

        string extension = string.Empty, questionFileName = string.Empty, answerFileName = string.Empty;
        if (dto.Status == QuestionStatus.Answered)
        {
            extension = ".png";
            var fileName = $"{dto.UserId}_{data!.LessonId}_{dto.QuestionId}{extension}";
            questionFileName = $"SQ_{fileName}";
            answerFileName = $"SA_{fileName}";
            await commonService.PictureConvert(model?.SimilarImage, questionFileName, AppOptions.SimilarQuestionPictureFolderPath);
            await commonService.PictureConvert(model?.AnswerImage, answerFileName, AppOptions.SimilarAnswerPictureFolderPath);
        }

        data!.UpdateUser = 1;
        data.UpdateDate = DateTime.Now;
        data.QuestionPicture = model?.QuestionText ?? string.Empty;
        data.ResponseQuestion = model?.SimilarQuestionText ?? string.Empty;
        data.ResponseQuestionFileName = questionFileName ?? string.Empty;
        data.ResponseQuestionExtension = extension ?? string.Empty;
        data.ResponseAnswer = model?.AnswerText ?? string.Empty;
        data.ResponseAnswerFileName = answerFileName ?? string.Empty;
        data.ResponseAnswerExtension = extension ?? string.Empty;
        data.Status = dto.Status;
        data.GainId = gain?.Id;
        data.RightOption = model?.RightOption?.FirstOrDefault();
        if (dto.Status != QuestionStatus.Answered)
        {
            data.TryCount++;
            data.Status = data.TryCount < AppOptions.AITryCount ? QuestionStatus.SendAgain : QuestionStatus.Error;
        }

        context.Similars.Update(data);
        await context.SaveChangesAsync();

        if (dto.Status == QuestionStatus.Answered)
            _ = notificationService.PushNotificationByUserId(new(Strings.Prepared, Strings.DynamicLessonQuestionPrepared.Format(data.Lesson?.Name), data.CreateUser, NotificationTypes.SimilarCreated, data.Id.ToString()));

        return true;
    }

    public async Task<bool> UpdateSimilarAnswer(SimilarTextResponseModel model, UpdateQuestionDto dto)
    {
        using var context = contextFactory.CreateDbContext();

        var data = await context.Similars
            .Include(x => x.Lesson)
            .FirstOrDefaultAsync(x => x.Id == dto.QuestionId && x.IsActive);
        await SimilarRules.SimilarQuestionShouldExists(data);

        GetGainModel? gain = null;
        if (dto.Status == QuestionStatus.Answered && model.GainName.IsNotEmpty())
            gain = await gainService.GetOrAddGain(new(model?.GainName, data!.LessonId, data.CreateUser, context));

        string extension = string.Empty, questionFileName = string.Empty, answerFileName = string.Empty;
        if (dto.Status == QuestionStatus.Answered)
        {
            extension = ".png";
            var fileName = $"{dto.UserId}_{data!.LessonId}_{dto.QuestionId}{extension}";
            questionFileName = $"SQ_{fileName}";
            answerFileName = $"SA_{fileName}";
            await commonService.TextToImage(model?.SimilarQuestionText, questionFileName, AppOptions.SimilarQuestionPictureFolderPath);
            await commonService.TextToImage(model?.AnswerText, answerFileName, AppOptions.SimilarAnswerPictureFolderPath);
        }

        data!.UpdateUser = 1;
        data.UpdateDate = DateTime.Now;
        data.ResponseQuestion = model?.SimilarQuestionText ?? string.Empty;
        data.ResponseQuestionFileName = questionFileName ?? string.Empty;
        data.ResponseQuestionExtension = extension ?? string.Empty;
        data.ResponseAnswer = model?.AnswerText ?? string.Empty;
        data.ResponseAnswerFileName = answerFileName ?? string.Empty;
        data.ResponseAnswerExtension = extension ?? string.Empty;
        data.Status = dto.Status;
        data.GainId = gain?.Id;
        data.RightOption = model?.RightOption?.FirstOrDefault();
        if (dto.Status != QuestionStatus.Answered)
        {
            data.TryCount++;
            data.Status = data.TryCount < AppOptions.AITryCount ? QuestionStatus.SendAgain : QuestionStatus.Error;
        }

        context.Similars.Update(data);
        await context.SaveChangesAsync();

        if (dto.Status == QuestionStatus.Answered)
            _ = notificationService.PushNotificationByUserId(new(Strings.Prepared, Strings.DynamicLessonQuestionPrepared.Format(data.Lesson?.Name), data.CreateUser, NotificationTypes.SimilarCreated, data.Id.ToString()));

        return true;
    }

    #endregion SimilarQuestion




    #region Quiz

    public async Task<string> AddQuiz(AddQuizModel model, CancellationToken cancellationToken)
    {
        await QuizRules.QuizQuestionShouldExists(model.QuestionList);
        await userRules.UserShouldExistsAndActiveById(model.UserId);

        using var context = contextFactory.CreateDbContext();

        var lessonName = await context.Lessons
            .AsNoTracking()
            .Where(x => x.Id == model.LessonId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync(cancellationToken);
        await LessonRules.LessonShouldExists(lessonName);

        var responses = await questionApi.GetSimilarForQuiz(new()
        {
            QuestionImages = model.QuestionList!,
            LessonName = lessonName,
            UserId = model.UserId
        });

        await QuizRules.QuizQuestionsShouldExists(responses.Questions);

        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var userId = 1;
            var date = DateTime.Now;
            var idPrefix = $"T-{model.LessonId}-{model.UserId}-";
            var quziId = $"{idPrefix}{date:HHmmssfff}";

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

            for (byte i = 0; i < responses.Questions.Count; i++)
            {
                var response = responses.Questions[i];
                var sortNo = (byte)(i + 1);

                var questionId = $"{quiz.Id}-{sortNo}";
                var extension = ".png";
                var fileName = $"{model.UserId}_{model.LessonId}_{questionId}{extension}";
                var questionFileName = $"TQ_{fileName}";
                var answerFileName = $"TA_{fileName}";
                await commonService.PictureConvert(response.SimilarImage, questionFileName, AppOptions.QuizQuestionPictureFolderPath);
                await commonService.PictureConvert(response.AnswerImage, answerFileName, AppOptions.QuizAnswerPictureFolderPath);

                var gain = await gainService.GetOrAddGain(new(response.GainName, model.LessonId, userId, context));

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
                    Question = response.QuestionText ?? string.Empty,
                    QuestionPictureFileName = questionFileName ?? string.Empty,
                    QuestionPictureExtension = extension ?? string.Empty,
                    Answer = response.AnswerText ?? string.Empty,
                    AnswerPictureFileName = answerFileName ?? string.Empty,
                    AnswerPictureExtension = extension ?? string.Empty,
                    RightOption = response.RightOption!.Trim()[0],
                    AnswerOption = null,
                    OptionCount = (byte)response.OptionCount,
                    GainId = gain?.Id
                });
            }

            await context.Quizzes.AddAsync(quiz, cancellationToken);
            await context.QuizQuestions.AddRangeAsync(questions, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _ = notificationService.PushNotificationByUserId(new(Strings.DynamicLessonTestPrepared.Format(lessonName), Strings.DynamicLessonTestPreparedForYou.Format(lessonName, quiz.Id), model.UserId, NotificationTypes.QuizCreated, quiz.Id));

            return quiz.Id;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new BusinessException(ex.Message);
        }
    }

    public async Task<string> AddQuizText(AddQuizModel model, CancellationToken cancellationToken)
    {
        await QuizRules.QuizQuestionShouldExists(model.QuestionList);
        await userRules.UserShouldExistsAndActiveById(model.UserId);

        using var context = contextFactory.CreateDbContext();

        var lessonName = await context.Lessons
            .AsNoTracking()
            .Where(x => x.Id == model.LessonId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync(cancellationToken);
        await LessonRules.LessonShouldExists(lessonName);

        var responses = await questionApi.GetSimilarTextForQuiz(new()
        {
            QuestionTexts = model.QuestionList!,
            VisualList = model.VisualList,
            LessonName = lessonName,
            UserId = model.UserId
        });

        await QuizRules.QuizQuestionsShouldExists(responses.Questions);

        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var userId = 1;
            var date = DateTime.Now;
            var idPrefix = $"T-{model.LessonId}-{model.UserId}-";
            var quziId = $"{idPrefix}{date:HHmmssfff}";

            await quizRules.QuizShouldNotExistsById(quziId);

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

            for (byte i = 0; i < responses.Questions.Count; i++)
            {
                var response = responses.Questions[i];
                var sortNo = (byte)(i + 1);

                var questionId = $"{quiz.Id}-{sortNo}";
                var extension = ".png";
                var fileName = $"{model.UserId}_{model.LessonId}_{questionId}{extension}";
                var questionFileName = $"TQ_{fileName}";
                var answerFileName = $"TA_{fileName}";
                await commonService.TextToImage(response.SimilarQuestionText, questionFileName, AppOptions.QuizQuestionPictureFolderPath);
                await commonService.TextToImage(response.AnswerText, answerFileName, AppOptions.QuizAnswerPictureFolderPath);

                var gain = await gainService.GetOrAddGain(new(response.GainName, model.LessonId, userId, context));

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
                    Question = response.QuestionText ?? string.Empty,
                    QuestionPictureFileName = questionFileName ?? string.Empty,
                    QuestionPictureExtension = extension ?? string.Empty,
                    Answer = response.AnswerText ?? string.Empty,
                    AnswerPictureFileName = answerFileName ?? string.Empty,
                    AnswerPictureExtension = extension ?? string.Empty,
                    RightOption = response.RightOption!.Trim()[0],
                    AnswerOption = null,
                    OptionCount = (byte)response.OptionCount,
                    GainId = gain?.Id
                });
            }

            await context.Quizzes.AddAsync(quiz, cancellationToken);
            await context.QuizQuestions.AddRangeAsync(questions, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _ = notificationService.PushNotificationByUserId(new(Strings.DynamicLessonTestPrepared.Format(lessonName), Strings.DynamicLessonTestPreparedForYou.Format(lessonName, quiz.Id), model.UserId, NotificationTypes.QuizCreated, quiz.Id));

            return quiz.Id;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new BusinessException(ex.Message);
        }
    }

    public async Task<bool> AddQuiz(bool timePass = false, CancellationToken cancellationToken = default)
    {
        try
        {
            AppStatics.SenderQuestionAllow = false;
            using var context = contextFactory.CreateDbContext();

            var questions = await context.Questions
                .AsNoTracking()
                .Include(u => u.User).ThenInclude(u => u!.School)
                .Include(u => u.Lesson)
                .Include(u => u.Gain)
                .Where(x => x.IsActive
                            && x.Status == QuestionStatus.Answered
                            && !x.SendForQuiz
                            && !x.ExcludeQuiz
                            && x.User!.IsActive
                            && x.Lesson!.IsActive
                            && x.Gain!.IsActive
                            && (x.User.SchoolId == null || x.User.School!.IsActive)
                            && (x.User.SchoolId == null || x.User.School!.LicenseEndDate.Date >= DateTime.Now.Date))
                .Select(x => new { x.Id, x.QuestionPictureBase64, x.QuestionPictureFileName, x.CreateUser, x.User!.SchoolId, x.LessonId, x.ExistsVisualContent })
                .ToListAsync(cancellationToken);

            if (questions.Count == 0) return false;

            var groupedQuestions = questions
                .GroupBy(x => new { x.SchoolId, x.LessonId, x.CreateUser })
                .Where(x => x.Count() > AppOptions.QuizMinimumQuestionLimit)
                .SelectMany(x => x.Take(AppOptions.QuizMinimumQuestionLimit).Select(q => new
                {
                    q.Id,
                    q.QuestionPictureFileName,
                    q.CreateUser,
                    q.SchoolId,
                    q.LessonId,
                    q.ExistsVisualContent
                }))
                .OrderBy(o => o.CreateUser).ThenBy(o => o.SchoolId).ThenBy(o => o.LessonId).ThenBy(o => o.CreateUser)
                .ToList();

            if (groupedQuestions.Count == 0) return false;

            var userGroups = groupedQuestions.GroupBy(q => q.CreateUser);

            foreach (var userGroup in userGroups)
            {
                var lessonGroups = userGroup.GroupBy(q => q.LessonId);

                foreach (var lessonGroup in lessonGroups)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return false;

                    if (DateTime.Now.Hour >= 7 && !timePass) return false;

                    var base64List = new List<string>();
                    var visualList = new List<bool>();

                    var questionsIds = lessonGroup.Select(x => x.Id).ToList();

                    foreach (var question in lessonGroup)
                    {
                        var filePath = Path.Combine(AppOptions.QuestionPictureFolderPath, question.QuestionPictureFileName!);
                        var fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
                        var base64String = Convert.ToBase64String(fileBytes);
                        base64List.Add(base64String);
                        visualList.Add(question.ExistsVisualContent);
                    }

                    var addQuizModel = new AddQuizModel
                    {
                        QuestionList = base64List,
                        LessonId = lessonGroup.Key,
                        UserId = userGroup.Key,
                        VisualList = visualList
                    };

                    try
                    {
                        await AddQuiz(addQuizModel, cancellationToken);

                        foreach (var questionId in questionsIds)
                        {
                            var questionUpdate = await context.Questions.FirstOrDefaultAsync(x => x.Id == questionId, cancellationToken);
                            questionUpdate!.SendForQuiz = true;
                            context.Questions.Update(questionUpdate);
                            await context.SaveChangesAsync(cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"{addQuizModel.UserId} - {addQuizModel.LessonId} - {ex.Message}");
                        continue;
                    }
                }
            }

            return true;
        }
        finally
        {
            AppStatics.SenderQuestionAllow = true;
        }
    }

    public async Task<bool> AddQuizText(bool timePass = false, CancellationToken cancellationToken = default)
    {
        try
        {
            AppStatics.SenderQuestionAllow = false;
            var changeDate = new DateTime(2024, 10, 19, 1, 30, 0);
            using var context = contextFactory.CreateDbContext();

            var questions = await context.Questions
                .AsNoTracking()
                .Include(u => u.User).ThenInclude(u => u!.School)
                .Include(u => u.Lesson)
                .Include(u => u.Gain)
                .Where(x => x.IsActive
                            && x.Status == QuestionStatus.Answered
                            && !x.SendForQuiz
                            && !x.ExcludeQuiz
                            && x.User!.IsActive
                            && x.Lesson!.IsActive
                            && x.Gain!.IsActive
                            && (x.User.SchoolId == null || x.User.School!.IsActive)
                            && (x.User.SchoolId == null || x.User.School!.LicenseEndDate.Date >= DateTime.Now.Date)
                            && x.CreateDate > changeDate)
                .Select(x => new { x.Id, x.QuestionPictureBase64, x.QuestionPictureFileName, x.CreateUser, x.User!.SchoolId, x.LessonId, x.ExistsVisualContent })
                .ToListAsync(cancellationToken);

            if (questions.Count == 0) return false;

            var groupedQuestions = questions
                .GroupBy(x => new { x.SchoolId, x.LessonId, x.CreateUser })
                .Where(x => x.Count() > AppOptions.QuizMinimumQuestionLimit)
                .SelectMany(x => x.Take(AppOptions.QuizMinimumQuestionLimit).Select(q => new
                {
                    q.Id,
                    q.QuestionPictureBase64,
                    q.QuestionPictureFileName,
                    q.CreateUser,
                    q.SchoolId,
                    q.LessonId,
                    q.ExistsVisualContent
                }))
                .OrderBy(o => o.CreateUser).ThenBy(o => o.SchoolId).ThenBy(o => o.LessonId).ThenBy(o => o.CreateUser)
                .ToList();

            if (groupedQuestions.Count == 0) return false;

            var userGroups = groupedQuestions.GroupBy(q => q.CreateUser);

            foreach (var userGroup in userGroups)
            {
                var lessonGroups = userGroup.GroupBy(q => q.LessonId);

                foreach (var lessonGroup in lessonGroups)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return false;

                    if (DateTime.Now.Hour >= 7 && !timePass) return false;

                    var questionsIds = lessonGroup.Select(x => x.Id).ToList();

                    var questionList = new List<string>();
                    var visualList = new List<bool>();

                    foreach (var question in lessonGroup)
                    {
                        var content = question.QuestionPictureBase64;
                        if (question.ExistsVisualContent)
                        {
                            var filePath = Path.Combine(AppOptions.QuestionPictureFolderPath, question.QuestionPictureFileName!);
                            content = await commonService.ImageToBase64(filePath);
                        }
                        questionList.Add(content!);
                        visualList.Add(question.ExistsVisualContent);
                    }

                    var addQuizModel = new AddQuizModel
                    {
                        QuestionList = questionList,
                        LessonId = lessonGroup.Key,
                        UserId = userGroup.Key,
                        VisualList = visualList
                    };

                    try
                    {
                        await AddQuizText(addQuizModel, cancellationToken);

                        foreach (var questionId in questionsIds)
                        {
                            var questionUpdate = await context.Questions.FirstOrDefaultAsync(x => x.Id == questionId, cancellationToken);
                            await QuestionRules.QuestionShouldExists(questionUpdate);
                            questionUpdate!.SendForQuiz = true;
                            context.Questions.Update(questionUpdate);
                            await context.SaveChangesAsync(cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"{addQuizModel.UserId} - {addQuizModel.LessonId} - {ex.Message}");
                        continue;
                    }
                }
            }

            return true;
        }
        finally
        {
            AppStatics.SenderQuestionAllow = true;
        }
    }

    #endregion Quiz
}

/*
    //public async Task SendForStatusSendAgain(CancellationToken cancellationToken)
    //{
    //    QuestionStatus[] status = [QuestionStatus.Waiting, QuestionStatus.Error, QuestionStatus.SendAgain];
    //    var questions = await questionDal.GetListAsync(
    //        predicate: x => (status.Contains(x.Status)
    //                         || (x.Status == QuestionStatus.Waiting && x.CreateDate < DateTime.Now.AddMinutes(1)))
    //                        && x.TryCount < AppOptions.AITryCount,
    //        include: x => x.Include(u => u.Lesson),
    //        enableTracking: false,
    //        cancellationToken: cancellationToken);

    //    foreach (var question in questions)
    //    {
    //        Console.WriteLine($"Question Id: {question.Id}");
    //        _ = await questionApi.AskQuestionOcrImage(new()
    //        {
    //            Id = question.Id,
    //            Base64 = question.QuestionPictureBase64,
    //            LessonName = question.Lesson.Name,
    //            UserId = question.CreateUser
    //        });
    //    }

    //    var similarQuestions = await similarQuestionDal.GetListAsync(
    //        predicate: x => (status.Contains(x.Status)
    //                         || (x.Status == QuestionStatus.Waiting && x.CreateDate < DateTime.Now.AddMinutes(1)))
    //                        && x.TryCount < AppOptions.AITryCount,
    //        include: x => x.Include(u => u.Lesson),
    //        enableTracking: false,
    //        cancellationToken: cancellationToken);

    //    foreach (var question in similarQuestions)
    //    {
    //        Console.WriteLine($"Similar Question Id: {question.Id}");
    //        _ = await questionApi.GetSimilarQuestion(new()
    //        {
    //            Id = question.Id,
    //            Base64 = question.QuestionPicture,
    //            LessonName = question.Lesson.Name,
    //            UserId = question.CreateUser
    //        });
    //    }
    //}
 */