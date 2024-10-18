using Business.Features.Questions.Models.Questions;
using Business.Services.CommonService;

namespace Business.Features.Questions.Rules;

public class QuestionRules(IQuestionDal questionDal,
                           ICommonService commonService) : IBusinessRule
{
    internal static Task QuestionShouldExists(GetQuestionModel model)
    {
        if (model == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Question);
        return Task.CompletedTask;
    }

    internal static Task QuestionShouldExists(Question question)
    {
        if (question == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Question);
        return Task.CompletedTask;
    }

    internal async Task QuestionLimitControl(byte lessonId)
    {
        if (commonService.HttpUserType == UserTypes.Administator) return;
        var count = await questionDal.CountOfRecordAsync(
                    enableTracking: false,
                    predicate: x => x.LessonId == lessonId
                                    && x.CreateUser == commonService.HttpUserId
                                    && x.Status != QuestionStatus.Error);

        if (count >= AppOptions.QuestionLimitForStudent) throw new BusinessException(Strings.QuestionLimitForStudentAndLesson);
    }

    internal async Task QuestionLimitControl()
    {
        var date = DateTime.Today;

        if (commonService.HttpUserType == UserTypes.Administator) return;
        var count = await questionDal.CountOfRecordAsync(
                    enableTracking: false,
                    predicate: x => x.CreateUser == commonService.HttpUserId
                                    && x.Status != QuestionStatus.Error
                                    && x.CreateDate >= DateTime.Today
                                    && x.CreateDate <= DateTime.Today.AddDays(1).AddMilliseconds(-1));

        if (count >= AppOptions.QuestionLimitForStudent) throw new BusinessException(Strings.QuestionLimitForStudent);
    }
}