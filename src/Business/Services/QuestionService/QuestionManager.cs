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

        var gain = await gainService.GetOrAddGain(new(model?.Kazanim, data.LessonId, data.CreateUser));

        data.UpdateDate = DateTime.Now;
        data.QuestionPictureBase64 = model?.Soru_OCR ?? string.Empty;
        data.AnswerText = model?.Cevap_Text ?? string.Empty;
        data.AnswerPictureFileName = string.Empty;
        data.AnswerPictureExtension = string.Empty;
        data.Status = dto.Status;
        data.TryCount = dto.Status == QuestionStatus.SendAgain ? data.TryCount++ : data.TryCount;
        data.GainId = gain?.Id;
        data.RightOption = model?.Cevap.FirstOrDefault();

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

        var gain = await gainService.GetOrAddGain(new(model?.Kazanim, data.LessonId, data.CreateUser));
        var filnename = await commonService.PictureConvert(model?.Cevap_Image, "response.png", AppOptions.AnswerPictureFolderPath);

        data.UpdateDate = DateTime.Now;
        data.QuestionPictureBase64 = model?.Soru_OCR ?? string.Empty;
        data.AnswerText = model?.Cevap_Text ?? string.Empty;
        data.AnswerPictureFileName = filnename.Item1;
        data.AnswerPictureExtension = filnename.Item2;
        data.Status = dto.Status;
        data.TryCount = dto.Status == QuestionStatus.SendAgain ? data.TryCount++ : data.TryCount;
        data.GainId = gain?.Id;
        data.RightOption = model?.Cevap.FirstOrDefault();

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

        var gain = await gainService.GetOrAddGain(new(model?.Kazanim, data.LessonId, data.CreateUser));

        var questionFileName = await commonService.PictureConvert(model?.Benzer_Image, "question.png", AppOptions.SimilarQuestionPictureFolderPath);
        var answerFileName = await commonService.PictureConvert(model?.Cevap_Image, "answer.png", AppOptions.SimilarAnswerPictureFolderPath);

        data.UpdateDate = DateTime.Now;
        data.QuestionPicture = model?.Soru_OCR ?? string.Empty;
        data.ResponseQuestion = model?.Benzer_Soru_Text ?? string.Empty;
        data.ResponseQuestionFileName = questionFileName.Item1;
        data.ResponseQuestionExtension = questionFileName.Item2;
        data.ResponseAnswer = model?.Cevap_Text ?? string.Empty;
        data.ResponseAnswerFileName = answerFileName.Item1;
        data.ResponseAnswerExtension = answerFileName.Item2;
        data.Status = dto.Status;
        data.TryCount = dto.Status == QuestionStatus.SendAgain ? data.TryCount++ : data.TryCount;
        data.GainId = gain?.Id;
        data.RightOption = model?.Cevap.FirstOrDefault();

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
}