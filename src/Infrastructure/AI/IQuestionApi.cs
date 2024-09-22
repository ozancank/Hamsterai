using Infrastructure.AI.Seduss.Models;

namespace Infrastructure.AI;

public interface IQuestionApi : IArtificialIntelligenceApi
{
    Task<QuestionTOResponseModel> AskQuestionOcr(string base64, Guid id, string lessonName);

    Task<QuestionITOResponseModel> AskQuestionOcrImage(string base64, Guid id, string lessonName);

    Task<SimilarResponseModel> GetSimilarQuestion(string base64, Guid id, string lessonName);
}