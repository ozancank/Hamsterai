using Business.Features.Questions.Rules;
using Business.Services.CommonService;
using Business.Services.NotificationService;
using Infrastructure.AI.Seduss.Models;

namespace Business.Services.QuestionService;

public class QuestionManager(ICommonService commonService,
                             IQuestionDal questionDal,
                             ISimilarQuestionDal similarQuestionDal,
                             INotificationService notificationService) : IQuestionService
{
    #region Question

    public async Task<bool> UpdateAnswer(string answer, string question, Guid questionId, QuestionStatus status)
    {
        var data = await questionDal.GetAsync(
            predicate: x => x.Id == questionId && x.IsActive,
            include: x => x.Include(u => u.Lesson));
        await QuestionRules.QuestionShouldExists(data);

        data.UpdateDate = DateTime.Now;
        data.QuestionPictureBase64 = question ?? string.Empty;
        data.AnswerText = answer;
        data.AnswerPictureFileName = string.Empty;
        data.AnswerPictureExtension = string.Empty;
        data.Status = status;

        await questionDal.UpdateAsync(data);

        _ = notificationService.PushNotificationByUserId(Strings.Answered, Strings.DynamicLessonQuestionAnswered.Format(data.Lesson.Name), data.CreateUser);

        return true;
    }

    public async Task<bool> UpdateAnswer(QuestionOcrModel model, Guid questionId, QuestionStatus status)
    {
        var data = await questionDal.GetAsync(
            predicate: x => x.Id == questionId && x.IsActive,
            include: x => x.Include(u => u.Lesson));
        await QuestionRules.QuestionShouldExists(data);

        data.UpdateDate = DateTime.Now;
        data.QuestionPictureBase64 = model.Soru_OCR ?? string.Empty;
        data.AnswerText = model.Cevap_Text ?? string.Empty;
        data.AnswerPictureFileName = string.Empty;
        data.AnswerPictureExtension = string.Empty;
        data.Status = status;

        await questionDal.UpdateAsync(data);

        _ = notificationService.PushNotificationByUserId(Strings.Answered, Strings.DynamicLessonQuestionAnswered.Format(data.Lesson.Name), data.CreateUser);

        return true;
    }

    public async Task<bool> UpdateAnswer(QuestionOcrImageModel model, Guid questionId, QuestionStatus status)
    {
        var data = await questionDal.GetAsync(
            predicate: x => x.Id == questionId && x.IsActive,
            include: x => x.Include(u => u.Lesson));
        await QuestionRules.QuestionShouldExists(data);

        var filnename = await commonService.PictureConvert(model.Cevap_Image, "response.png", AppOptions.AnswerPictureFolderPath);

        data.UpdateDate = DateTime.Now;
        data.QuestionPictureBase64 = model.Soru_OCR ?? string.Empty;
        data.AnswerText = model.Cevap_Text ?? string.Empty;
        data.AnswerPictureFileName = filnename.Item1;
        data.AnswerPictureExtension = filnename.Item2;
        data.Status = status;

        await questionDal.UpdateAsync(data);

        _ = notificationService.PushNotificationByUserId(Strings.Answered, Strings.DynamicLessonQuestionAnswered.Format(data.Lesson.Name), data.CreateUser);

        return true;
    }

    #endregion Question

    #region SimilarQuestion

    public async Task<bool> UpdateSimilarAnswer(SimilarModel model, Guid questionId, QuestionStatus status)
    {
        var data = await similarQuestionDal.GetAsync(
            predicate: x => x.Id == questionId && x.IsActive,
            include: x => x.Include(u => u.Lesson));
        await SimilarRules.SimilarQuestionShouldExists(data);

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

        await similarQuestionDal.UpdateAsync(data);

        _ = notificationService.PushNotificationByUserId(Strings.Prepared, Strings.DynamicLessonQuestionPrepared.Format(data.Lesson.Name), data.CreateUser);

        return true;
    }

    #endregion SimilarQuestion
}