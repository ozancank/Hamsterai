using Business.Features.Questions.Models.Quizzes;
using Infrastructure.AI.Seduss.Dtos;
using Infrastructure.AI.Seduss.Models;

namespace Business.Services.QuestionService;

public interface IQuestionService : IBusinessService
{
    #region Question

    Task<bool> UpdateAnswer(QuestionTOResponseModel model, UpdateQuestionDto dto);

    Task<bool> UpdateAnswer(QuestionITOResponseModel model, UpdateQuestionDto dto);

    Task<bool> UpdateAnswer(QuestionTextResponseModel model, UpdateQuestionDto dto);

    #endregion Question

    #region SimilarQuestion

    Task<bool> UpdateSimilarAnswer(SimilarResponseModel model, UpdateQuestionDto dto);

    Task<bool> UpdateSimilarAnswer(SimilarTextResponseModel model, UpdateQuestionDto dto);

    #endregion SimilarQuestion

    Task SendForStatusSendAgain(CancellationToken cancellationToken);

    #region Quiz

    Task<string> AddQuiz(AddQuizModel model, CancellationToken cancellationToken);

    Task<string> AddQuizText(AddQuizModel model, CancellationToken cancellationToken);

    Task<bool> AddQuiz(bool timePass = false, CancellationToken cancellationToken = default);

    Task<bool> AddQuizText(bool timePass = false, CancellationToken cancellationToken = default);

    #endregion Quiz
}