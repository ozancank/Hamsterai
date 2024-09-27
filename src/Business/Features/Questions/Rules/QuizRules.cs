using Business.Features.Questions.Models.Quizzes;
using Infrastructure.AI.Seduss.Models;

namespace Business.Features.Questions.Rules;

public class QuizRules : IBusinessRule
{
    internal static Task QuizShouldExists(GetQuizModel model)
    {
        if (model == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Quiz);
        return Task.CompletedTask;
    }

    internal static Task QuizShouldExists(Quiz quiz)
    {
        if (quiz == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Quiz);
        return Task.CompletedTask;
    }

    internal static Task QuizQuestionShouldExists(List<string> questionImages)
    {
        if (questionImages == null || questionImages.Count <= 0) throw new BusinessException(Strings.DynamicNotEmpty, Strings.Question);
        return Task.CompletedTask;
    }

    internal static Task QuizQuestionsShouldExists(List<SimilarResponseModel> responses)
    {
        if (responses == null || responses.Count <= 0) throw new BusinessException(Strings.DynamicNotEmpty, Strings.Question);
        return Task.CompletedTask;
    }
}