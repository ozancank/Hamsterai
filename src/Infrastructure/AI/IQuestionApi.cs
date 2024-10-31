using Infrastructure.AI.Seduss.Models;

namespace Infrastructure.AI;

public interface IQuestionApi : IArtificialIntelligenceApi
{
    Task<QuestionITOResponseModel> AskQuestionOcrImage(QuestionApiModel model);

    [Obsolete(message: "Currently Not Available")]
    Task<QuestionTextResponseModel> AskQuestionText(QuestionApiModel model);

    [Obsolete(message: "Currently Not Available")]
    Task<QuestionVisualResponseModel> AskQuestionVisual(QuestionApiModel model);

    Task<SimilarResponseModel> GetSimilar(QuestionApiModel model);

    [Obsolete(message: "Currently Not Available")]
    Task<SimilarTextResponseModel> GetSimilarText(QuestionApiModel model);

    Task<QuizResponseModel> GetQuizQuestions(QuizApiModel model);

    Task<QuizResponseModel> GetSimilarForQuiz(QuizApiModel model);

    Task<QuizTextResponseModel> GetSimilarTextForQuiz(QuizApiModel model);
}