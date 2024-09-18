using Domain.Constants;
using Infrastructure.AI.Seduss.Models;

namespace Infrastructure.Constants;

public class InfrastructureDelegates
{
    public delegate Task<bool> UpdateQuestionDelegate(string answer, string question, Guid questionId, QuestionStatus status);
    public static UpdateQuestionDelegate UpdateQuestion { get; set; }

    public delegate Task<bool> UpdateQuestionOcrDelegate(QuestionOcrModel model, Guid questionId, QuestionStatus status);
    public static UpdateQuestionOcrDelegate UpdateQuestionOcr { get; set; }

    public delegate Task<bool> UpdateQuestionOcrImageDelegate(QuestionOcrImageModel model, Guid questionId, QuestionStatus status);
    public static UpdateQuestionOcrImageDelegate UpdateQuestionOcrImage { get; set; }

    public delegate Task<bool> UpdateSimilarQuestionAnswerDelegate(SimilarModel model, Guid questionId, QuestionStatus status);
    public static UpdateSimilarQuestionAnswerDelegate UpdateSimilarQuestionAnswer { get; set; }

}