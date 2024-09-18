using Business.Features.Questions.Models.Questions;

namespace Business.Features.Questions.Rules;

public class QuestionRules : IBusinessRule
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
}