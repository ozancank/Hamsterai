using Infrastructure.AI.Seduss.Models;

namespace Business.Services.QuestionService;

public interface IQuestionService : IBusinessService
{
    #region Question

    Task<bool> UpdateAnswer(QuestionTOResponseModel model, Guid questionId, QuestionStatus status);

    Task<bool> UpdateAnswer(QuestionITOResponseModel model, Guid questionId, QuestionStatus status);

    #endregion Question

    #region SimilarQuestion

    Task<bool> UpdateSimilarAnswer(SimilarResponseModel model, Guid questionId, QuestionStatus status);

    #endregion SimilarQuestion

    Task SendForStatusSendAgain(CancellationToken cancellationToken);
}