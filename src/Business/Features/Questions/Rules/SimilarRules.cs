using Business.Features.Questions.Models.Similars;

namespace Business.Features.Questions.Rules;

public class SimilarRules : IBusinessRule
{
    internal static Task SimilarQuestionShouldExists(GetSimilarModel model)
    {
        if (model == null) throw new BusinessException(Strings.DynamicNotFound, Strings.SimilarQuestion);
        return Task.CompletedTask;
    }

    internal static Task SimilarQuestionShouldExists(SimilarQuestion SimilarQuestion)
    {
        if (SimilarQuestion == null) throw new BusinessException(Strings.DynamicNotFound, Strings.SimilarQuestion);
        return Task.CompletedTask;
    }
}