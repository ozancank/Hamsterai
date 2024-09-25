using Infrastructure.AI.Seduss.Dtos;
using Infrastructure.AI.Seduss.Models;

namespace Business.Services.QuestionService;

public interface IQuestionService : IBusinessService
{
    #region Question

    Task<bool> UpdateAnswer(QuestionTOResponseModel model, UpdateQuestionDto dto);

    Task<bool> UpdateAnswer(QuestionITOResponseModel model, UpdateQuestionDto dto);

    #endregion Question

    #region SimilarQuestion

    Task<bool> UpdateSimilarAnswer(SimilarResponseModel model, UpdateQuestionDto dto);

    #endregion SimilarQuestion

    Task SendForStatusSendAgain(CancellationToken cancellationToken);

    #region Quiz

    Task<bool> AddQuiz(List<string> questions);

    #endregion Quiz

}