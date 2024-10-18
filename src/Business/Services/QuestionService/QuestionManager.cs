using Business.Features.Lessons.Models.Gains;
using Business.Features.Lessons.Rules;
using Business.Features.Questions.Models.Quizzes;
using Business.Features.Questions.Rules;
using Business.Features.Users.Rules;
using Business.Services.CommonService;
using Business.Services.GainService;
using Business.Services.NotificationService;
using DataAccess.EF;
using Infrastructure.AI;
using Infrastructure.AI.Seduss.Dtos;
using Infrastructure.AI.Seduss.Models;
using OCK.Core.Logging.Serilog;

namespace Business.Services.QuestionService;

public class QuestionManager(ICommonService commonService,
                             IQuestionDal questionDal,
                             ISimilarDal similarDal,
                             INotificationService notificationService,
                             IGainService gainService,
                             IQuestionApi questionApi,
                             IQuizDal quizDal,
                             IQuizQuestionDal quizQuestionDal,
                             ILessonDal lessonDal,
                             IDbContextFactory<HamsteraiDbContext> _factory,
                             UserRules userRules,
                             LoggerServiceBase logger) : IQuestionService
{
    #region Question

    public async Task<bool> UpdateAnswer(QuestionTOResponseModel model, UpdateQuestionDto dto)
    {
        var data = await questionDal.GetAsync(
            predicate: x => x.Id == dto.QuestionId && x.IsActive,
            include: x => x.Include(u => u.Lesson));
        await QuestionRules.QuestionShouldExists(data);

        GetGainModel gain = null;
        if (dto.Status == QuestionStatus.Answered && model.GainName.IsNotEmpty())
            gain = await gainService.GetOrAddGain(new(model?.GainName, data.LessonId, data.CreateUser));

        data.UpdateUser = 1;
        data.UpdateDate = DateTime.Now;
        data.QuestionPictureBase64 = model?.QuestionText ?? string.Empty;
        data.AnswerText = model?.AnswerText ?? string.Empty;
        data.AnswerPictureFileName = string.Empty;
        data.AnswerPictureExtension = string.Empty;
        data.Status = dto.Status;
        data.GainId = gain?.Id;
        data.RightOption = model?.RightOption.FirstOrDefault();
        if (dto.Status != QuestionStatus.Answered) data.TryCount++;

        await questionDal.UpdateAsync(data);

        if (dto.Status == QuestionStatus.Answered)
            _ = notificationService.PushNotificationByUserId(new(Strings.Answered, Strings.DynamicLessonQuestionAnswered.Format(data.Lesson.Name), data.CreateUser, NotificationTypes.QuestionAnswered, dto.QuestionId.ToString()));

        return true;
    }

    public async Task<bool> UpdateAnswer(QuestionITOResponseModel model, UpdateQuestionDto dto)
    {
        var data = await questionDal.GetAsync(
            predicate: x => x.Id == dto.QuestionId && x.IsActive,
            include: x => x.Include(u => u.Lesson));
        await QuestionRules.QuestionShouldExists(data);

        GetGainModel gain = null;
        if (dto.Status == QuestionStatus.Answered && model.GainName.IsNotEmpty())
            gain = await gainService.GetOrAddGain(new(model?.GainName, data.LessonId, data.CreateUser));

        string extension = string.Empty;
        string fileName = string.Empty;
        if (dto.Status == QuestionStatus.Answered)
        {
            extension = ".png";
            fileName = $"A_{dto.UserId}_{data.LessonId}_{dto.QuestionId}{extension}";
            await commonService.TextToImage(model?.AnswerText, fileName, AppOptions.AnswerPictureFolderPath);
        }

        data.UpdateUser = 1;
        data.UpdateDate = DateTime.Now;
        data.QuestionPictureBase64 = model?.QuestionText ?? string.Empty;
        data.AnswerText = model?.AnswerText ?? string.Empty;
        data.AnswerPictureFileName = fileName;
        data.AnswerPictureExtension = extension;
        data.Status = dto.Status;
        data.GainId = gain?.Id;
        data.RightOption = model?.RightOption.FirstOrDefault();
        if (dto.Status != QuestionStatus.Answered) data.TryCount++;

        await questionDal.UpdateAsync(data);

        if (dto.Status == QuestionStatus.Answered)
            _ = notificationService.PushNotificationByUserId(new(Strings.Answered, Strings.DynamicLessonQuestionAnswered.Format(data.Lesson.Name), data.CreateUser, NotificationTypes.QuestionAnswered, dto.QuestionId.ToString()));

        return true;
    }

    public async Task<bool> UpdateAnswer(QuestionTextResponseModel model, UpdateQuestionDto dto)
    {
        using var context = _factory.CreateDbContext();

        var data = await context.Questions
            .Include(x => x.Lesson)
            .FirstOrDefaultAsync(x => x.Id == dto.QuestionId && x.IsActive);
        await QuestionRules.QuestionShouldExists(data);

        GetGainModel gain = null;
        if (dto.Status == QuestionStatus.Answered && model.GainName.IsNotEmpty())
            gain = await gainService.GetOrAddGain(new(model?.GainName, data.LessonId, data.CreateUser, context));

        string extension = string.Empty, fileName = string.Empty;
        if (dto.Status == QuestionStatus.Answered)
        {
            extension = ".png";
            fileName = $"A_{dto.UserId}_{data.LessonId}_{dto.QuestionId}{extension}";
            await commonService.TextToImage(model?.AnswerText, fileName, AppOptions.AnswerPictureFolderPath);
        }

        data.UpdateUser = 1;
        data.UpdateDate = DateTime.Now;
        data.AnswerText = model?.AnswerText ?? string.Empty;
        data.AnswerPictureFileName = fileName;
        data.AnswerPictureExtension = extension;
        data.Status = dto.Status;
        data.GainId = gain?.Id;
        data.RightOption = model?.RightOption.FirstOrDefault();
        if (dto.Status != QuestionStatus.Answered) data.TryCount++;

        context.Questions.Update(data);
        await context.SaveChangesAsync();

        if (dto.Status == QuestionStatus.Answered)
            _ = notificationService.PushNotificationByUserId(new(Strings.Answered, Strings.DynamicLessonQuestionAnswered.Format(data.Lesson.Name), data.CreateUser, NotificationTypes.QuestionAnswered, dto.QuestionId.ToString()));

        return true;
    }

    #endregion Question

    #region SimilarQuestion

    public async Task<bool> UpdateSimilarAnswer(SimilarResponseModel model, UpdateQuestionDto dto)
    {
        var data = await similarDal.GetAsync(
            predicate: x => x.Id == dto.QuestionId && x.IsActive,
            include: x => x.Include(u => u.Lesson));
        await SimilarRules.SimilarQuestionShouldExists(data);

        GetGainModel gain = null;
        if (dto.Status == QuestionStatus.Answered && model.GainName.IsNotEmpty())
            gain = await gainService.GetOrAddGain(new(model?.GainName, data.LessonId, data.CreateUser));

        string extension = null, questionFileName = null, answerFileName = null;
        if (dto.Status == QuestionStatus.Answered)
        {
            extension = ".png";
            var fileName = $"{dto.UserId}_{data.LessonId}_{dto.QuestionId}{extension}";
            questionFileName = $"SQ_{fileName}";
            answerFileName = $"SA_{fileName}";
            await commonService.PictureConvert(model?.SimilarImage, questionFileName, AppOptions.SimilarQuestionPictureFolderPath);
            await commonService.PictureConvert(model?.AnswerImage, answerFileName, AppOptions.SimilarAnswerPictureFolderPath);
        }

        data.UpdateUser = 1;
        data.UpdateDate = DateTime.Now;
        data.QuestionPicture = model?.QuestionText ?? string.Empty;
        data.ResponseQuestion = model?.SimilarQuestionText ?? string.Empty;
        data.ResponseQuestionFileName = questionFileName ?? string.Empty;
        data.ResponseQuestionExtension = extension ?? string.Empty;
        data.ResponseAnswer = model?.AnswerText ?? string.Empty;
        data.ResponseAnswerFileName = answerFileName ?? string.Empty;
        data.ResponseAnswerExtension = extension;
        data.Status = dto.Status;
        data.GainId = gain?.Id;
        data.RightOption = model?.RightOption.FirstOrDefault();
        if (dto.Status != QuestionStatus.Answered) data.TryCount++;

        await similarDal.UpdateAsync(data);

        if (dto.Status == QuestionStatus.Answered)
            _ = notificationService.PushNotificationByUserId(new(Strings.Prepared, Strings.DynamicLessonQuestionPrepared.Format(data.Lesson.Name), data.CreateUser, NotificationTypes.SimilarCreated, data.Id.ToString()));

        return true;
    }

    public async Task<bool> UpdateSimilarAnswer(SimilarTextResponseModel model, UpdateQuestionDto dto)
    {
        using var context = _factory.CreateDbContext();

        var data = await context.Similars
            .Include(x => x.Lesson)
            .FirstOrDefaultAsync(x => x.Id == dto.QuestionId && x.IsActive);
        await SimilarRules.SimilarQuestionShouldExists(data);

        GetGainModel gain = null;
        if (dto.Status == QuestionStatus.Answered && model.GainName.IsNotEmpty())
            gain = await gainService.GetOrAddGain(new(model?.GainName, data.LessonId, data.CreateUser, context));

        string extension = null, questionFileName = null, answerFileName = null;
        if (dto.Status == QuestionStatus.Answered)
        {
            extension = ".png";
            var fileName = $"{dto.UserId}_{data.LessonId}_{dto.QuestionId}{extension}";
            questionFileName = $"SQ_{fileName}";
            answerFileName = $"SA_{fileName}";
            await commonService.TextToImage(model?.SimilarQuestionText, fileName, AppOptions.SimilarQuestionPictureFolderPath);
            await commonService.TextToImage(model?.AnswerText, fileName, AppOptions.SimilarAnswerPictureFolderPath);
        }

        data.UpdateUser = 1;
        data.UpdateDate = DateTime.Now;
        data.ResponseQuestion = model?.SimilarQuestionText ?? string.Empty;
        data.ResponseQuestionFileName = questionFileName ?? string.Empty;
        data.ResponseQuestionExtension = extension ?? string.Empty;
        data.ResponseAnswer = model?.AnswerText ?? string.Empty;
        data.ResponseAnswerFileName = answerFileName ?? string.Empty;
        data.ResponseAnswerExtension = extension;
        data.Status = dto.Status;
        data.GainId = gain?.Id;
        data.RightOption = model?.RightOption.FirstOrDefault();
        if (dto.Status != QuestionStatus.Answered) data.TryCount++;

        context.Similars.Update(data);
        await context.SaveChangesAsync();

        if (dto.Status == QuestionStatus.Answered)
            _ = notificationService.PushNotificationByUserId(new(Strings.Prepared, Strings.DynamicLessonQuestionPrepared.Format(data.Lesson.Name), data.CreateUser, NotificationTypes.SimilarCreated, data.Id.ToString()));

        return true;
    }

    #endregion SimilarQuestion

    public SemaphoreSlim SenderSemaphore = new(AppOptions.SenderCapacity);

    public async Task SendForStatusSendAgain(CancellationToken cancellationToken)
    {
        AppStatics.SenderQuestionAllow = false;
        var changeDate = new DateTime(2024, 10, 19, 1, 30, 0);
        try
        {
            QuestionStatus[] status = [QuestionStatus.Waiting, QuestionStatus.Error, QuestionStatus.SendAgain];
            var questions = await questionDal.GetListAsync(
                predicate: x => status.Contains(x.Status) && x.TryCount < AppOptions.AITryCount && x.CreateDate > changeDate,
                include: x => x.Include(u => u.Lesson),
                enableTracking: false,
                cancellationToken: cancellationToken);

            var similarQuestions = await similarDal.GetListAsync(
                predicate: x => status.Contains(x.Status) && x.TryCount < AppOptions.AITryCount && x.CreateDate > changeDate,
                include: x => x.Include(u => u.Lesson),
                enableTracking: false,
                cancellationToken: cancellationToken);

            var allQuestions = questions.Cast<object>().Concat(similarQuestions.Cast<object>()).ToList();

            if (allQuestions.Count == 0) return;

            var tasks = allQuestions.Select(async question =>
            {
                await SenderSemaphore.WaitAsync(cancellationToken);
                try
                {
                    if (question is Question q)
                    {
                        await questionApi.AskQuestionText(new()
                        {
                            Id = q.Id,
                            QuestionText = q.QuestionPictureBase64,
                            LessonName = q.Lesson.Name,
                            UserId = q.CreateUser,
                            ExcludeQuiz = q.ExcludeQuiz
                        });
                    }
                    else if (question is Similar sm)
                    {
                        await questionApi.GetSimilarText(new()
                        {
                            Id = sm.Id,
                            QuestionText = sm.QuestionPicture,
                            LessonName = sm.Lesson.Name,
                            UserId = sm.CreateUser,
                            ExcludeQuiz = sm.ExcludeQuiz
                        });
                    }
                }
                finally
                {
                    SenderSemaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
        }
        finally
        {
            AppStatics.SenderQuestionAllow = true;
        }
    }

    #region Quiz

    public async Task<string> AddQuiz(AddQuizModel model, CancellationToken cancellationToken)
    {
        await QuizRules.QuizQuestionShouldExists(model.Base64List);
        await userRules.UserShouldExistsAndActiveById(model.UserId);

        var lessonName = await lessonDal.GetAsync(
            predicate: x => x.Id == model.LessonId,
            enableTracking: false,
            selector: x => x.Name,
            cancellationToken: cancellationToken);
        await LessonRules.LessonShouldExists(lessonName);

        var responses = await questionApi.GetSimilarForQuiz(new()
        {
            QuestionImages = model.Base64List,
            LessonName = lessonName,
            UserId = model.UserId
        });

        await QuizRules.QuizQuestionsShouldExists(responses.Questions);

        var result = await quizDal.ExecuteWithTransactionAsync(async () =>
        {
            var userId = 1;
            var date = DateTime.Now;
            var idPrefix = $"T-{model.LessonId}-{model.UserId}-";
            var maxId = await quizDal.Query().AsNoTracking().Where(x => x.Id.StartsWith(idPrefix)).OrderBy(x => x.Id).Select(x => x.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            var nextId = Convert.ToInt32(maxId?[idPrefix.Length..] ?? "0") + 1;
            var quziId = $"{idPrefix}{nextId}";
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

                var gain = await gainService.GetOrAddGain(new(response.GainName, model.LessonId, userId));

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
                    Question = response.QuestionText,
                    QuestionPictureFileName = questionFileName,
                    QuestionPictureExtension = extension,
                    Answer = response.AnswerText,
                    AnswerPictureFileName = answerFileName,
                    AnswerPictureExtension = extension,
                    RightOption = response.RightOption.Trim()[0],
                    AnswerOption = null,
                    OptionCount = (byte)response.OptionCount,
                    GainId = gain.Id
                });
            }

            await quizDal.AddAsync(quiz, cancellationToken: cancellationToken);
            await quizQuestionDal.AddRangeAsync(questions, cancellationToken: cancellationToken);

            return quiz.Id;
        }, cancellationToken: cancellationToken);

        _ = await notificationService.PushNotificationByUserId(new(Strings.DynamicLessonTestPrepared.Format(lessonName), Strings.DynamicLessonTestPreparedForYou.Format(lessonName, result), model.UserId, NotificationTypes.QuizCreated, result));

        return result;
    }

    public async Task<string> AddQuizText(AddQuizModel model, CancellationToken cancellationToken)
    {
        await QuizRules.QuizQuestionShouldExists(model.QuestionList);
        await userRules.UserShouldExistsAndActiveById(model.UserId);

        var lessonName = await lessonDal.GetAsync(
            predicate: x => x.Id == model.LessonId,
            enableTracking: false,
            selector: x => x.Name,
            cancellationToken: cancellationToken);
        await LessonRules.LessonShouldExists(lessonName);

        var responses = await questionApi.GetSimilarTextForQuiz(new()
        {
            QuestionTexts = model.QuestionList,
            LessonName = lessonName,
            UserId = model.UserId
        });

        await QuizRules.QuizQuestionsShouldExists(responses.Questions);

        var result = await quizDal.ExecuteWithTransactionAsync(async () =>
        {
            var userId = 1;
            var date = DateTime.Now;
            var idPrefix = $"T-{model.LessonId}-{model.UserId}-";
            var maxId = await quizDal.Query().AsNoTracking().Where(x => x.Id.StartsWith(idPrefix)).OrderBy(x => x.Id).Select(x => x.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            var nextId = Convert.ToInt32(maxId?[idPrefix.Length..] ?? "0") + 1;
            var quziId = $"{idPrefix}{nextId}";
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

                var gain = await gainService.GetOrAddGain(new(response.GainName, model.LessonId, userId));

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
                    Question = response.QuestionText,
                    QuestionPictureFileName = questionFileName,
                    QuestionPictureExtension = extension,
                    Answer = response.AnswerText,
                    AnswerPictureFileName = answerFileName,
                    AnswerPictureExtension = extension,
                    RightOption = response.RightOption.Trim()[0],
                    AnswerOption = null,
                    OptionCount = (byte)response.OptionCount,
                    GainId = gain.Id
                });
            }

            await quizDal.AddAsync(quiz, cancellationToken: cancellationToken);
            await quizQuestionDal.AddRangeAsync(questions, cancellationToken: cancellationToken);

            return quiz.Id;
        }, cancellationToken: cancellationToken);

        _ = await notificationService.PushNotificationByUserId(new(Strings.DynamicLessonTestPrepared.Format(lessonName), Strings.DynamicLessonTestPreparedForYou.Format(lessonName, result), model.UserId, NotificationTypes.QuizCreated, result));

        return result;
    }

    public async Task<bool> AddQuiz(bool timePass = false, CancellationToken cancellationToken = default)
    {
        try
        {
            AppStatics.SenderQuestionAllow = false;

            var questions = await questionDal.GetListAsync(
                enableTracking: false,
                predicate: x => x.IsActive
                                && x.Status == QuestionStatus.Answered
                                && !x.SendForQuiz
                                && x.User.IsActive
                                && x.Lesson.IsActive
                                && x.Gain.IsActive
                                && (x.User.SchoolId == null || x.User.School.IsActive)
                                && (x.User.SchoolId == null || x.User.School.LicenseEndDate.Date >= DateTime.Now.Date),
                include: x => x.Include(u => u.User).ThenInclude(u => u.School)
                               .Include(u => u.Lesson)
                               .Include(u => u.Gain),
                selector: x => new { x.Id, x.QuestionPictureFileName, x.CreateUser, x.User.SchoolId, x.LessonId },
                cancellationToken: cancellationToken);

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
                    q.LessonId
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
                    var questionsIds = lessonGroup.Select(x => x.Id).ToList();

                    foreach (var question in lessonGroup)
                    {
                        var filePath = Path.Combine(AppOptions.QuestionPictureFolderPath, question.QuestionPictureFileName);
                        var fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
                        var base64String = Convert.ToBase64String(fileBytes);
                        base64List.Add(base64String);
                    }

                    var addQuizModel = new AddQuizModel
                    {
                        Base64List = base64List,
                        LessonId = lessonGroup.Key,
                        UserId = userGroup.Key
                    };

                    try
                    {
                        await AddQuiz(addQuizModel, cancellationToken);

                        foreach (var questionId in questionsIds)
                        {
                            var questionUpdate = await questionDal.GetAsync(x => x.Id == questionId, cancellationToken: cancellationToken);
                            questionUpdate.SendForQuiz = true;
                            await questionDal.UpdateAsync(questionUpdate, cancellationToken: cancellationToken);
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

            var questions = await questionDal.GetListAsync(
                enableTracking: false,
                predicate: x => x.IsActive
                                && x.Status == QuestionStatus.Answered
                                && !x.SendForQuiz
                                && !x.ExcludeQuiz
                                && x.User.IsActive
                                && x.Lesson.IsActive
                                && x.Gain.IsActive
                                && (x.User.SchoolId == null || x.User.School.IsActive)
                                && (x.User.SchoolId == null || x.User.School.LicenseEndDate.Date >= DateTime.Now.Date)
                                && x.CreateDate > changeDate,
                include: x => x.Include(u => u.User).ThenInclude(u => u.School)
                               .Include(u => u.Lesson)
                               .Include(u => u.Gain),
                selector: x => new { x.Id, x.QuestionPictureBase64, x.CreateUser, x.User.SchoolId, x.LessonId },
                cancellationToken: cancellationToken);

            if (questions.Count == 0) return false;

            var groupedQuestions = questions
                .GroupBy(x => new { x.SchoolId, x.LessonId, x.CreateUser })
                .Where(x => x.Count() > AppOptions.QuizMinimumQuestionLimit)
                .SelectMany(x => x.Take(AppOptions.QuizMinimumQuestionLimit).Select(q => new
                {
                    q.Id,
                    q.QuestionPictureBase64,
                    q.CreateUser,
                    q.SchoolId,
                    q.LessonId
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

                    var questionList = lessonGroup.Select(x => x.QuestionPictureBase64).ToList();

                    var addQuizModel = new AddQuizModel
                    {
                        QuestionList = questionList,
                        LessonId = lessonGroup.Key,
                        UserId = userGroup.Key
                    };

                    try
                    {
                        await AddQuizText(addQuizModel, cancellationToken);

                        foreach (var questionId in questionsIds)
                        {
                            var questionUpdate = await questionDal.GetAsync(x => x.Id == questionId, cancellationToken: cancellationToken);
                            questionUpdate.SendForQuiz = true;
                            await questionDal.UpdateAsync(questionUpdate, cancellationToken: cancellationToken);
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