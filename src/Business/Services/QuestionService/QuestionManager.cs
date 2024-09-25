using Business.Features.Questions.Rules;
using Business.Services.CommonService;
using Business.Services.GainService;
using Business.Services.NotificationService;
using Infrastructure.AI;
using Infrastructure.AI.Seduss.Dtos;
using Infrastructure.AI.Seduss.Models;

namespace Business.Services.QuestionService;

public class QuestionManager(ICommonService commonService,
                             IQuestionDal questionDal,
                             ISimilarQuestionDal similarQuestionDal,
                             INotificationService notificationService,
                             IGainService gainService,
                             IQuestionApi questionApi) : IQuestionService
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
        var questions = await questionDal.GetListAsync(
            predicate: x => (x.Status == QuestionStatus.SendAgain
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
            predicate: x => (x.Status == QuestionStatus.SendAgain
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

    public Task<bool> AddQuiz(List<string> questions)
    {
        throw new NotImplementedException();
    }

    #endregion Quiz
}