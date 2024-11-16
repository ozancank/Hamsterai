using Infrastructure.AI.Seduss.Models;

namespace Infrastructure.AI;

public interface IQuestionApi : IArtificialIntelligenceApi
{
    Task<QuestionITOResponseModel> AskQuestionOcrImage(QuestionApiModel model);

    Task<SimilarResponseModel> GetSimilar(QuestionApiModel model);

    Task<QuizResponseModel> GetQuizQuestions(QuizApiModel model);

    Task<QuizResponseModel> GetSimilarForQuiz(QuizApiModel model);
}