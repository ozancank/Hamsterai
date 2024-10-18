using Infrastructure.AI.Seduss.Models;

namespace Infrastructure.AI;

public interface IQuestionApi : IArtificialIntelligenceApi
{
    Task<QuestionTOResponseModel> AskQuestionOcr(QuestionApiModel model);

    Task<QuestionITOResponseModel> AskQuestionOcrImage(QuestionApiModel model);

    Task<QuestionTextResponseModel> AskQuestionText(QuestionApiModel model);

    Task<SimilarResponseModel> GetSimilar(QuestionApiModel model);

    Task<SimilarTextResponseModel> GetSimilarText(QuestionApiModel model);

    Task<QuizResponseModel> GetQuizQuestions(QuizApiModel model);

    Task<QuizResponseModel> GetSimilarForQuiz(QuizApiModel model);

    Task<QuizTextResponseModel> GetSimilarTextForQuiz(QuizApiModel model);
}