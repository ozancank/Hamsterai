using DataAccess.EF;
using Infrastructure.AI.Models;

namespace Application.Features.Questions.Rules;

public class QuizRules(IQuizDal quizDal, IQuizQuestionDal quizQuestionDal) : IBusinessRule
{
    internal static Task QuizShouldExists(object? model)
    {
        if (model == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Quiz);
        return Task.CompletedTask;
    }

    internal static Task QuizQuestionShouldExists(List<string> questionImages)
    {
        if (questionImages == null || questionImages.Count <= 0) throw new BusinessException(Strings.DynamicNotEmpty, Strings.Question);
        return Task.CompletedTask;
    }

    internal static Task QuizQuestionShouldExists(List<Similar> questionImages)
    {
        if (questionImages == null || questionImages.Count <= 0) throw new BusinessException(Strings.DynamicNotEmpty, Strings.Question);
        return Task.CompletedTask;
    }

    internal static Task QuizQuestionsShouldExists(List<SimilarResponseModel> responses)
    {
        if (responses == null || responses.Count <= 0) throw new BusinessException(Strings.DynamicNotEmpty, Strings.Question);
        return Task.CompletedTask;
    }

    internal async Task QuizShouldNotExistsById(string id)
    {
        var control = await quizDal.IsExistsAsync(predicate: x => PostgresqlFunctions.TrLower(x.Id) == PostgresqlFunctions.TrLower(id), enableTracking: false);
        if (control) throw new BusinessException(Strings.DynamicExists, $"{Strings.Quiz} Id");
    }

    internal async Task QuizQuestionShouldNotExistsById(string id)
    {
        var control = await quizQuestionDal.IsExistsAsync(predicate: x => PostgresqlFunctions.TrLower(x.Id) == PostgresqlFunctions.TrLower(id), enableTracking: false);
        if (control) throw new BusinessException(Strings.DynamicExists, $"{Strings.Quiz}-{Strings.Question} Id");
    }

    internal static Task RightOptionShouldExists(char rightOption)
    {
        if (!AppStatics.OptionChars.Contains(rightOption)) throw new BusinessException(Strings.DynamicBetween, [Strings.Options, "A", "E"]);
        return Task.CompletedTask;
    }
}