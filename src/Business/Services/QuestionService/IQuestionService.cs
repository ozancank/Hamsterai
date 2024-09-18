using Infrastructure.AI.Seduss.Models;

namespace Business.Services.QuestionService;

public interface IQuestionService : IBusinessService
{
    #region Question

    Task<bool> UpdateAnswer(string answer, string question, Guid questionId, QuestionStatus status);

    Task<bool> UpdateAnswer(QuestionOcrModel model, Guid questionId, QuestionStatus status);

    Task<bool> UpdateAnswer(QuestionOcrImageModel model, Guid questionId, QuestionStatus status);

    #endregion Question

    #region SimilarQuestion

    Task<bool> UpdateSimilarAnswer(SimilarModel model, Guid questionId, QuestionStatus status);

    #endregion SimilarQuestion
}