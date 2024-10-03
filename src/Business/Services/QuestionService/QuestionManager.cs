using Business.Features.Lessons.Rules;
using Business.Features.Questions.Models.Quizzes;
using Business.Features.Questions.Rules;
using Business.Features.Users.Rules;
using Business.Services.CommonService;
using Business.Services.GainService;
using Business.Services.NotificationService;
using Infrastructure.AI;
using Infrastructure.AI.Seduss.Dtos;
using Infrastructure.AI.Seduss.Models;
using Newtonsoft.Json;

namespace Business.Services.QuestionService;

public class QuestionManager(ICommonService commonService,
                             IQuestionDal questionDal,
                             ISimilarQuestionDal similarQuestionDal,
                             INotificationService notificationService,
                             IGainService gainService,
                             IQuestionApi questionApi,
                             IQuizDal quizDal,
                             IQuizQuestionDal quizQuestionDal,
                             ILessonDal lessonDal,
                             UserRules userRules) : IQuestionService
{
    #region Question

    public async Task<bool> UpdateAnswer(QuestionTOResponseModel model, UpdateQuestionDto dto)
    {
        var data = await questionDal.GetAsync(
            predicate: x => x.Id == dto.QuestionId && x.IsActive,
            include: x => x.Include(u => u.Lesson));
        await QuestionRules.QuestionShouldExists(data);

        var gain = await gainService.GetOrAddGain(new(model?.GainName, data.LessonId, data.CreateUser));

        data.UpdateDate = DateTime.Now;
        data.QuestionPictureBase64 = model?.QuestionText ?? string.Empty;
        data.AnswerText = model?.AnswerText ?? string.Empty;
        data.AnswerPictureFileName = string.Empty;
        data.AnswerPictureExtension = string.Empty;
        data.Status = dto.Status;
        data.TryCount = dto.Status == QuestionStatus.SendAgain ? data.TryCount++ : data.TryCount;
        data.GainId = gain?.Id;
        data.RightOption = model?.RightOption.FirstOrDefault();

        await questionDal.UpdateAsync(data);

        if (dto.Status == QuestionStatus.Answered)
            _ = notificationService.PushNotificationByUserId(Strings.Answered, Strings.DynamicLessonQuestionAnswered.Format(data.Lesson.Name), data.CreateUser);

        return true;
    }

    public async Task<bool> UpdateAnswer(QuestionITOResponseModel model, UpdateQuestionDto dto)
    {
        var data = await questionDal.GetAsync(
            predicate: x => x.Id == dto.QuestionId && x.IsActive,
            include: x => x.Include(u => u.Lesson));
        await QuestionRules.QuestionShouldExists(data);

        var gain = await gainService.GetOrAddGain(new(model?.GainName, data.LessonId, data.CreateUser));

        var date = DateTime.Now;
        var extension = ".png";
        var fileName = $"A_{dto.UserId}_{data.LessonId}_{dto.QuestionId}{extension}";
        await commonService.PictureConvert(model?.AnswerImage, fileName, AppOptions.AnswerPictureFolderPath);

        data.UpdateDate = DateTime.Now;
        data.QuestionPictureBase64 = model?.QuestionText ?? string.Empty;
        data.AnswerText = model?.AnswerText ?? string.Empty;
        data.AnswerPictureFileName = fileName;
        data.AnswerPictureExtension = extension;
        data.Status = dto.Status;
        data.TryCount = dto.Status == QuestionStatus.SendAgain ? data.TryCount++ : data.TryCount;
        data.GainId = gain?.Id;
        data.RightOption = model?.RightOption.FirstOrDefault();

        await questionDal.UpdateAsync(data);

        if (dto.Status == QuestionStatus.Answered)
            _ = notificationService.PushNotificationByUserId(Strings.Answered, Strings.DynamicLessonQuestionAnswered.Format(data.Lesson.Name), data.CreateUser);

        return true;
    }

    #endregion Question

    #region SimilarQuestion

    public async Task<bool> UpdateSimilarAnswer(SimilarResponseModel model, UpdateQuestionDto dto)
    {
        var data = await similarQuestionDal.GetAsync(
            predicate: x => x.Id == dto.QuestionId && x.IsActive,
            include: x => x.Include(u => u.Lesson));
        await SimilarRules.SimilarQuestionShouldExists(data);

        var gain = await gainService.GetOrAddGain(new(model?.GainName, data.LessonId, data.CreateUser));

        var date = DateTime.Now;
        var extension = ".png";
        var fileName = $"{dto.UserId}_{data.LessonId}_{dto.QuestionId}{extension}";
        var questionFileName = $"SQ_{fileName}";
        var answerFileName = $"SA_{fileName}";
        await commonService.PictureConvert(model?.SimilarImage, questionFileName, AppOptions.SimilarQuestionPictureFolderPath);
        await commonService.PictureConvert(model?.AnswerImage, answerFileName, AppOptions.SimilarAnswerPictureFolderPath);

        data.UpdateDate = date;
        data.QuestionPicture = model?.QuestionText ?? string.Empty;
        data.ResponseQuestion = model?.SimilarQuestionText ?? string.Empty;
        data.ResponseQuestionFileName = questionFileName;
        data.ResponseQuestionExtension = extension;
        data.ResponseAnswer = model?.AnswerText ?? string.Empty;
        data.ResponseAnswerFileName = answerFileName;
        data.ResponseAnswerExtension = extension;
        data.Status = dto.Status;
        data.TryCount = dto.Status == QuestionStatus.SendAgain ? data.TryCount++ : data.TryCount;
        data.GainId = gain?.Id;
        data.RightOption = model?.RightOption.FirstOrDefault();

        await similarQuestionDal.UpdateAsync(data);

        if (dto.Status == QuestionStatus.Answered)
            _ = notificationService.PushNotificationByUserId(Strings.Prepared, Strings.DynamicLessonQuestionPrepared.Format(data.Lesson.Name), data.CreateUser);

        return true;
    }

    #endregion SimilarQuestion

    public async Task SendForStatusSendAgain(CancellationToken cancellationToken)
    {
        QuestionStatus[] status = [QuestionStatus.SendAgain, QuestionStatus.Error];
        var questions = await questionDal.GetListAsync(
            predicate: x => (status.Contains(x.Status)
                             || (x.Status == QuestionStatus.Waiting && x.CreateDate < DateTime.Now.AddMinutes(1)))
                            && x.TryCount < AppOptions.AITryCount,
            include: x => x.Include(u => u.Lesson),
            enableTracking: false,
            cancellationToken: cancellationToken);

        foreach (var question in questions)
        {
            _ = await questionApi.AskQuestionOcrImage(new()
            {
                Id = question.Id,
                Base64 = question.QuestionPictureBase64,
                LessonName = question.Lesson.Name,
                UserId = question.CreateUser
            });
        }

        var similarQuestions = await similarQuestionDal.GetListAsync(
            predicate: x => (status.Contains(x.Status)
                             || (x.Status == QuestionStatus.Waiting && x.CreateDate < DateTime.Now.AddMinutes(1)))
                            && x.TryCount < AppOptions.AITryCount,
            include: x => x.Include(u => u.Lesson),
            enableTracking: false,
            cancellationToken: cancellationToken);

        foreach (var question in similarQuestions)
        {
            _ = await questionApi.GetSimilarQuestion(new()
            {
                Id = question.Id,
                Base64 = question.QuestionPicture,
                LessonName = question.Lesson.Name,
                UserId = question.CreateUser
            });
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

        var responses = await questionApi.GetQuizQuestions(new()
        {
            QuestionImages = model.Base64List,
            LessonName = lessonName,
            UserId = model.UserId
        });
        await QuizRules.QuizQuestionsShouldExists(responses.Questions);

        var userId = commonService.HttpUserId;
        var date = DateTime.Now;
        var count = await quizDal.CountOfRecordAsync(predicate: x => x.UserId == model.UserId && x.LessonId == model.LessonId, enableTracking: false, cancellationToken: cancellationToken) + 1;
        var quiz = new Quiz
        {
            Id = $"T-{model.LessonId}-{model.UserId}-{count}",
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

            var id = $"{quiz.Id}-{sortNo}";
            var extension = ".png";
            var fileName = $"{model.UserId}_{model.LessonId}_{id}{extension}";
            var questionFileName = $"TQ_{fileName}";
            var answerFileName = $"TA_{fileName}";
            await commonService.PictureConvert(response.SimilarImage, questionFileName, AppOptions.QuizQuestionPictureFolderPath);
            await commonService.PictureConvert(response.AnswerImage, answerFileName, AppOptions.QuizAnswerPictureFolderPath);

            var gain = await gainService.GetOrAddGain(new(response.GainName, model.LessonId, userId));

            questions.Add(new()
            {
                Id = id,
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

        await quizDal.ExecuteWithTransactionAsync(async () =>
        {
            await quizDal.AddAsync(quiz, cancellationToken: cancellationToken);
            await quizQuestionDal.AddRangeAsync(questions, cancellationToken: cancellationToken);
        }, cancellationToken: cancellationToken);

        return quiz.Id;
    }

    public async Task<bool> AddQuiz(CancellationToken cancellationToken)
    {
        var questions = await questionDal.GetListAsync(
            enableTracking: false,
            predicate: x => x.IsActive
                            && x.Status == QuestionStatus.Answered
                            && !x.SendForQuiz
                            && x.User.IsActive
                            && x.Lesson.IsActive
                            && x.Gain.IsActive
                            && (x.User.SchoolId == null || x.User.School.IsActive)
                            && (x.User.SchoolId == null || x.User.School.LicenseEndDate >= DateTime.Now),
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

                if (DateTime.Now.Hour >= 7) return false;

                var base64List = new List<string>();

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

                await AddQuiz(addQuizModel, cancellationToken);
            }
        }

        return true;
    }

    #endregion Quiz
}