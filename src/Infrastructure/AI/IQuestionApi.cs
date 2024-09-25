using Infrastructure.AI.Seduss.Models;

namespace Infrastructure.AI;

public interface IQuestionApi : IArtificialIntelligenceApi
{
    Task<QuestionTOResponseModel> AskQuestionOcr(QuestionApiModel model);

    Task<QuestionITOResponseModel> AskQuestionOcrImage(QuestionApiModel model);

    Task<SimilarResponseModel> GetSimilarQuestion(QuestionApiModel model);

    Task<QuizResponseModel> GetQuizQuestions(QuizApiModel model);
}