using Business.Features.Questions.Commands.Questions;
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
        var count = await questionDal.CountOfRecordAsync(
                    enableTracking: false,
                    predicate: x => x.LessonId == lessonId
                                    && x.CreateUser == commonService.HttpUserId
                                    && x.Status != QuestionStatus.Error);

        if (count >= AppOptions.QuestionLimitForStudent) throw new BusinessException(Strings.QuestionLimitForStudent);
    }
}