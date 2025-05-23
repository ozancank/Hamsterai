﻿using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using Infrastructure.OCR.Models;

namespace Application.Features.Questions.Rules;

public class QuestionRules(IQuestionDal questionDal,
                           IUserDal userDal,
                           IPackageUserDal packageUserDal,
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
                     && x.CreateDate <= date.AddMonths(1).AddMilliseconds(-1)).CountAsync();

        if (count >= AppOptions.QuestionMonthLimitForStudent) throw new BusinessException(Strings.QuestionLimitForStudent);
    }

    internal static Task OCRShouldBeFilled(OcrResponseModel ocr)
    {
        if (ocr == null) throw new BusinessException(Strings.NoResponseFromOCR);
        if (ocr.Text.IsEmpty()) throw new BusinessException(Strings.NoResponseFromOCR);
        return Task.CompletedTask;
    }

    internal async Task UserShouldHaveCredit(long userId)
    {
        var user = await userDal.GetAsync(
            enableTracking: false,
            predicate: x => x.Id == userId);

        var newUserId = user.Id;

        if (user.Type == UserTypes.Administator) return;
        if (user.Type == UserTypes.School || user.Type == UserTypes.Teacher || user.Type == UserTypes.Student)
        {
            newUserId = await userDal.GetAsync(
                enableTracking: false,
                predicate: x => x.SchoolId == user.SchoolId && x.Type == UserTypes.School,
                selector: x => x.Id);
        }

        var packageUsers = await packageUserDal.GetListAsync(
            enableTracking: false,
            predicate: x => x.UserId == newUserId && x.EndDate > DateTime.Now,
            selector: x => x.QuestionCredit);

        var totalCredit = packageUsers?.Sum() ?? 0;

        var questionCount = await questionDal.CountOfRecordAsync(
            enableTracking: false,
            predicate: x => x.CreateUser == user.Id && AppStatics.QuestionStatusesForCredit.Contains(x.Status));

        var remainingCredit = totalCredit - questionCount;

        if (remainingCredit <= 0) throw new BusinessException(Strings.NoQuestionCredit);
    }

    internal static Task QuestionTypeShouldBeText(QuestionType type)
    {
        if(!AppStatics.QuestionTypesOnlyText.Contains(type)) throw new BusinessException(Strings.QuestionTypeText);
        return Task.CompletedTask;
    }

    internal static Task QuestionStatusShouldBeAnswered(QuestionStatus status)
    {
        if (status != QuestionStatus.Answered) throw new BusinessException(Strings.DynamicMustBe, [$"{Strings.Question} {Strings.OfStatus} {Strings.Answered}"]);
        return Task.CompletedTask;
    }
}