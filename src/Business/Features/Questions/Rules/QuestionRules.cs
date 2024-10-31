using Business.Features.Questions.Models.Questions;
using Business.Services.CommonService;
using Infrastructure.OCR.Models;

namespace Business.Features.Questions.Rules;

public class QuestionRules(IQuestionDal questionDal,
                           ICommonService commonService) : IBusinessRule
{
    internal static Task QuestionShouldExists(object? model)
    {
        if (model == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Question);
        return Task.CompletedTask;
    }

    internal async Task QuestionLimitControl()
    {
        var date = DateTime.Today;

        if (commonService.HttpUserType == UserTypes.Administator) return;
        var count = await questionDal.Query().AsNoTracking()
            .Where(x => x.CreateUser == commonService.HttpUserId
                                    && x.Status != QuestionStatus.Error
                                    && x.CreateDate >= date
                                    && x.CreateDate <= date.AddDays(1).AddMilliseconds(-1)).CountAsync();

        if (count >= AppOptions.QuestionLimitForStudent) throw new BusinessException(Strings.QuestionLimitForStudent);
    }

    internal static Task OCRShouldBeFilled(OcrResponseModel ocr)
    {
        if (ocr == null) throw new BusinessException(Strings.NoResponseFromOCR);
        if (ocr.Text.IsEmpty()) throw new BusinessException(Strings.NoResponseFromOCR);
        return Task.CompletedTask;
    }
}