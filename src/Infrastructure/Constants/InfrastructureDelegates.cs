using Domain.Constants;
using Infrastructure.AI.Seduss.Models;

namespace Infrastructure.Constants;

public class InfrastructureDelegates
{
    public delegate Task<bool> UpdateQuestionOcrDelegate(QuestionTOResponseModel model, Guid questionId, QuestionStatus status);
    public static UpdateQuestionOcrDelegate UpdateQuestionOcr { get; set; }

    public delegate Task<bool> UpdateQuestionOcrImageDelegate(QuestionITOResponseModel model, Guid questionId, QuestionStatus status);
    public static UpdateQuestionOcrImageDelegate UpdateQuestionOcrImage { get; set; }

    public delegate Task<bool> UpdateSimilarQuestionAnswerDelegate(SimilarResponseModel model, Guid questionId, QuestionStatus status);
    public static UpdateSimilarQuestionAnswerDelegate UpdateSimilarQuestionAnswer { get; set; }

}