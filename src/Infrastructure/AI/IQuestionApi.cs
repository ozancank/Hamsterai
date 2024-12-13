using Infrastructure.AI.Models;

namespace Infrastructure.AI;

public interface IQuestionApi : IArtificialIntelligenceApi
{
    Task<QuestionResponseModel> AskQuestionWithImage(QuestionApiModel model, CancellationToken cancellationToken = default);

    Task<QuestionResponseModel> AskQuestionWithText(QuestionApiModel model, CancellationToken cancellationToken = default);

    Task<QuestionResponseModel> AskOcr(QuestionApiModel model, CancellationToken cancellationToken = default);

    Task<SimilarResponseModel> GetSimilar(QuestionApiModel model, CancellationToken cancellationToken = default);

    Task<GainResponseModel> GetGain(QuestionApiModel model, CancellationToken cancellationToken = default);

    Task<bool> IsExistsVisualContent(QuestionApiModel model, CancellationToken cancellationToken = default);

    //Task<QuizResponseModel> GetQuizQuestions(QuizApiModel model);

    //Task<QuizResponseModel> GetSimilarForQuiz(QuizApiModel model);
}