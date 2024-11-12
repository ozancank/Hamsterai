using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using Infrastructure.OCR.Models;

namespace Application.Features.Questions.Rules;

public class QuestionRules(IQuestionDal questionDal,
                           IUserDal userDal,
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

    internal async Task UserShouldHaveCredit(long userId)
    {
        var totalCredit = await userDal.GetAsync(
            predicate: x => x.Id == userId,
            enableTracking: false,
            selector: x => x.PackageCredit + x.AddtionalCredit);

        if (totalCredit <= 0) throw new BusinessException(Strings.NoQuestionCredit);
    }
}