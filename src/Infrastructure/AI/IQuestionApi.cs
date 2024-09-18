using Infrastructure.AI.Seduss.Models;

namespace Infrastructure.AI;

public interface IQuestionApi : IArtificialIntelligenceApi
{
    Task<string> AskQuestion(string base64, Guid id);

    Task<QuestionOcrModel> AskQuestionOcr(string base64, Guid id);

    Task<QuestionOcrImageModel> AskQuestionOcrImage(string base64, Guid id);

    Task<SimilarModel> GetSimilarQuestion(string base64, Guid id);
}