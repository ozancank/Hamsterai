using Business.Features.Questions.Models.Similars;
using Business.Services.CommonService;
using DataAccess.EF.Concrete;
using Infrastructure.OCR.Models;

namespace Business.Features.Questions.Rules;

public class SimilarRules(ISimilarDal similarDal,
                          ICommonService commonService) : IBusinessRule
{
    internal static Task SimilarQuestionShouldExists(object? model)
    {
        if (model == null) throw new BusinessException(Strings.DynamicNotFound, Strings.SimilarQuestion);
        return Task.CompletedTask;
    }

    internal async Task SimilarLimitControl()
    {
        var date = DateTime.Today;

        if (commonService.HttpUserType == UserTypes.Administator) return;
        var count = await similarDal.Query().AsNoTracking()
            .Where(x => x.CreateUser == commonService.HttpUserId
                        && x.Status != QuestionStatus.Error
                        && x.CreateDate >= date
                        && x.CreateDate <= date.AddDays(1).AddMilliseconds(-1)).CountAsync();

        if (count >= AppOptions.SimilarLimitForStudent) throw new BusinessException(Strings.SimilarLimitForStudent);
    }

    internal static Task OCRShouldBeFilled(OcrResponseModel ocr)
    {
        if (ocr == null) throw new BusinessException(Strings.NoResponseFromOCR);
        if (ocr.Text.IsEmpty()) throw new BusinessException(Strings.NoResponseFromOCR);
        return Task.CompletedTask;
    }
}