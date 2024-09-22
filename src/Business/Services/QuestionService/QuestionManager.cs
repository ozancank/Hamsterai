using Business.Features.Lessons.Models.Gain;
using Business.Features.Questions.Rules;
using Business.Services.CommonService;
using Business.Services.GainService;
using Business.Services.NotificationService;
using Infrastructure.AI;
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

    public async Task<bool> UpdateAnswer(QuestionTOResponseModel model, Guid questionId, QuestionStatus status)
    {
        var data = await questionDal.GetAsync(
            predicate: x => x.Id == questionId && x.IsActive,
            include: x => x.Include(u => u.Lesson));
        await QuestionRules.QuestionShouldExists(data);

        var gain = await gainService.GetOrAddGainModelAsync(new(model.Kazanim, data.LessonId));

        data.UpdateDate = DateTime.Now;
        data.QuestionPictureBase64 = model.Soru_OCR ?? string.Empty;
        data.AnswerText = model.Cevap_Text ?? string.Empty;
        data.AnswerPictureFileName = string.Empty;
        data.AnswerPictureExtension = string.Empty;
        data.Status = status;
        data.TryCount = status == QuestionStatus.SendAgain ? data.TryCount++ : data.TryCount;
        data.GainId = gain?.Id;

        await questionDal.UpdateAsync(data);

        _ = notificationService.PushNotificationByUserId(Strings.Answered, Strings.DynamicLessonQuestionAnswered.Format(data.Lesson.Name), data.CreateUser);

        return true;
    }

    public async Task<bool> UpdateAnswer(QuestionITOResponseModel model, Guid questionId, QuestionStatus status)
    {
        var data = await questionDal.GetAsync(
            predicate: x => x.Id == questionId && x.IsActive,
            include: x => x.Include(u => u.Lesson));
        await QuestionRules.QuestionShouldExists(data);

        GetGainModel gain = null;
        if (model != null)
            gain = await gainService.GetOrAddGainModelAsync(new(model.Kazanim, data.LessonId));

        var filnename = await commonService.PictureConvert(model.Cevap_Image, "response.png", AppOptions.AnswerPictureFolderPath);

        data.UpdateDate = DateTime.Now;
        data.QuestionPictureBase64 = model.Soru_OCR ?? string.Empty;
        data.AnswerText = model.Cevap_Text ?? string.Empty;
        data.AnswerPictureFileName = filnename.Item1;
        data.AnswerPictureExtension = filnename.Item2;
        data.Status = status;
        data.TryCount = status == QuestionStatus.SendAgain ? data.TryCount++ : data.TryCount;
        data.GainId = gain?.Id;

        await questionDal.UpdateAsync(data);

        _ = notificationService.PushNotificationByUserId(Strings.Answered, Strings.DynamicLessonQuestionAnswered.Format(data.Lesson.Name), data.CreateUser);

        return true;
    }

    #endregion Question

    #region SimilarQuestion

    public async Task<bool> UpdateSimilarAnswer(SimilarResponseModel model, Guid questionId, QuestionStatus status)
    {
        var data = await similarQuestionDal.GetAsync(
            predicate: x => x.Id == questionId && x.IsActive,
            include: x => x.Include(u => u.Lesson));
        await SimilarRules.SimilarQuestionShouldExists(data);

        var gain = await gainService.GetOrAddGainModelAsync(new(model.Kazanim, data.LessonId));

        var questionFileName = await commonService.PictureConvert(model.Benzer_Image, "question.png", AppOptions.SimilarQuestionPictureFolderPath);
        var answerFileName = await commonService.PictureConvert(model.Cevap_Image, "answer.png", AppOptions.SimilarAnswerPictureFolderPath);

        data.UpdateDate = DateTime.Now;
        data.QuestionPicture = model?.Soru_OCR ?? string.Empty;
        data.ResponseQuestion = model?.Benzer_Soru_Text ?? string.Empty;
        data.ResponseQuestionFileName = questionFileName.Item1;
        data.ResponseQuestionExtension = questionFileName.Item2;
        data.ResponseAnswer = model?.Cevap_Text ?? string.Empty;
        data.ResponseAnswerFileName = answerFileName.Item1;
        data.ResponseAnswerExtension = answerFileName.Item2;
        data.Status = status;
        data.TryCount = status == QuestionStatus.SendAgain ? data.TryCount++ : data.TryCount;
        data.GainId = gain?.Id;

        await similarQuestionDal.UpdateAsync(data);

        _ = notificationService.PushNotificationByUserId(Strings.Prepared, Strings.DynamicLessonQuestionPrepared.Format(data.Lesson.Name), data.CreateUser);

        return true;
    }

    #endregion SimilarQuestion

    public async Task SendForStatusSendAgain(CancellationToken cancellationToken)
    {
        var questions = await questionDal.GetListAsync(
            predicate: x => x.Status == QuestionStatus.SendAgain && x.TryCount < AppOptions.AITryCount,
            include: x => x.Include(u => u.Lesson),
            enableTracking: false,
            cancellationToken: cancellationToken);

        foreach (var question in questions)
        {
            _ = await questionApi.AskQuestionOcrImage(question.QuestionPictureBase64, question.Id, question.Lesson.Name);
        }

        var similarQuestions = await similarQuestionDal.GetListAsync(
            predicate: x => x.Status == QuestionStatus.SendAgain && x.TryCount < AppOptions.AITryCount,
            include: x => x.Include(u => u.Lesson),
            enableTracking: false,
            cancellationToken: cancellationToken);

        foreach (var question in similarQuestions)
        {
            _ = await questionApi.GetSimilarQuestion(question.QuestionPicture, question.Id, question.Lesson.Name);
        }
    }
}