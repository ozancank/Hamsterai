using Business.Features.Questions.Models.Similars;
using Business.Services.CommonService;
using Infrastructure.OCR.Models;

namespace Business.Features.Questions.Rules;

public class SimilarRules(ISimilarDal similarQuestionDal,
                          ICommonService commonService) : IBusinessRule
{
    internal static Task SimilarQuestionShouldExists(GetSimilarModel model)
    {
        if (model == null) throw new BusinessException(Strings.DynamicNotFound, Strings.SimilarQuestion);
        return Task.CompletedTask;
    }

    internal static Task SimilarQuestionShouldExists(Similar similar)
    {
        if (similar == null) throw new BusinessException(Strings.DynamicNotFound, Strings.SimilarQuestion);
        return Task.CompletedTask;
    }

    internal async Task SimilarLimitControl(byte lessonId)
    {
        if (commonService.HttpUserType == UserTypes.Administator) return;
        var count = await similarQuestionDal.CountOfRecordAsync(
                    enableTracking: false,
                    predicate: x => x.LessonId == lessonId
                                    && x.CreateUser == commonService.HttpUserId
                                    && x.Status != QuestionStatus.Error);

        if (count >= AppOptions.SimilarLimitForStudent) throw new BusinessException(Strings.SimilarLimitForStudentAndLesson);
    }

    internal async Task SimilarLimitControl()
    {
        if (commonService.HttpUserType == UserTypes.Administator) return;
        var count = await similarQuestionDal.CountOfRecordAsync(
                    enableTracking: false,
                    predicate: x => x.CreateUser == commonService.HttpUserId
                                    && x.Status != QuestionStatus.Error);

        if (count >= AppOptions.SimilarLimitForStudent) throw new BusinessException(Strings.SimilarLimitForStudent);
    }

    internal static Task OCRShouldBeFilled(OcrResponseModel ocr)
    {
        if (ocr == null) throw new BusinessException(Strings.NoResponseFromOCR);
        if (ocr.Text.IsEmpty()) throw new BusinessException(Strings.NoResponseFromOCR);
        return Task.CompletedTask;
    }
}